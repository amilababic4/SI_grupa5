using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using System.Security.Claims;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Korisnici modul — Pregled članova, profil (MVC)
    /// </summary>
    [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
    public class KorisnikController : Controller
    {
        private readonly IKorisnikRepository _korisnikRepository;
        private readonly IClanarinaRepository _clanarinaRepository;

        public KorisnikController(
            IKorisnikRepository korisnikRepository,
            IClanarinaRepository clanarinaRepository)
        {
            _korisnikRepository = korisnikRepository;
            _clanarinaRepository = clanarinaRepository;
        }

        public async Task<IActionResult> Index(string? pretraga = null, bool prikaziDeaktivirane = false)
        {
            var korisnici = await _korisnikRepository.GetAllAsync();

            var upit = korisnici
                .Where(k => string.Equals(k.Uloga?.Naziv, RoleNames.Clan, StringComparison.OrdinalIgnoreCase));

            if (!prikaziDeaktivirane)
                upit = upit.Where(k => string.Equals(k.Status, "aktivan", StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(pretraga))
            {
                var term = pretraga.Trim().ToLowerInvariant();
                upit = upit.Where(k =>
                    (k.Ime != null && k.Ime.ToLowerInvariant().Contains(term)) ||
                    (k.Prezime != null && k.Prezime.ToLowerInvariant().Contains(term)) ||
                    (k.Email != null && k.Email.ToLowerInvariant().Contains(term)));
            }

            var clanovi = upit
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .Select(k => new KorisnikDto
                {
                    Id = k.Id,
                    Ime = k.Ime,
                    Prezime = k.Prezime,
                    Email = k.Email,
                    Uloga = k.Uloga?.Naziv ?? string.Empty,
                    Status = k.Status,
                    DatumKreiranja = k.DatumKreiranja
                })
                .ToList();

            ViewBag.Pretraga = pretraga;
            ViewBag.PrikaziDeaktivirane = prikaziDeaktivirane;
            ViewBag.UkupnoRezultata = clanovi.Count;

            return View(clanovi);
        }

        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<IActionResult> IndexBibliotekar(string? pretraga = null, bool prikaziDeaktivirane = false)
        {
            var korisnici = await _korisnikRepository.GetAllAsync();

            var upit = korisnici
                .Where(k => string.Equals(k.Uloga?.Naziv, RoleNames.Bibliotekar, StringComparison.OrdinalIgnoreCase));

            if (!prikaziDeaktivirane)
                upit = upit.Where(k => string.Equals(k.Status, "aktivan", StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(pretraga))
            {
                var term = pretraga.Trim().ToLowerInvariant();
                upit = upit.Where(k =>
                    (k.Ime != null && k.Ime.ToLowerInvariant().Contains(term)) ||
                    (k.Prezime != null && k.Prezime.ToLowerInvariant().Contains(term)) ||
                    (k.Email != null && k.Email.ToLowerInvariant().Contains(term)));
            }

            var bibliotekari = upit
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .Select(k => new KorisnikDto
                {
                    Id = k.Id,
                    Ime = k.Ime,
                    Prezime = k.Prezime,
                    Email = k.Email,
                    Uloga = k.Uloga?.Naziv ?? string.Empty,
                    Status = k.Status,
                    DatumKreiranja = k.DatumKreiranja
                })
                .ToList();

            ViewBag.Pretraga = pretraga;
            ViewBag.PrikaziDeaktivirane = prikaziDeaktivirane;
            ViewBag.UkupnoRezultata = bibliotekari.Count;

            return View(bibliotekari);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Profil()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim, out int id))
                return RedirectToAction("Login", "Auth");

            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik == null)
                return NotFound();

            var clanarina = await _clanarinaRepository.GetByKorisnikAsync(id);
            if (clanarina != null)
            {
                ViewBag.Clanarina = new ClanarinaDto
                {
                    Id = clanarina.Id,
                    DatumPocetka = clanarina.DatumPocetka,
                    DatumIsteka = clanarina.DatumIsteka
                };
            }

            ViewBag.JeMojProfil = true;
            return View("Profil", korisnik);
        }

        [HttpGet]
        public async Task<IActionResult> ProfilClana(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik == null)
                return NotFound();

            var clanarina = await _clanarinaRepository.GetByKorisnikAsync(id);
            if (clanarina != null)
            {
                ViewBag.Clanarina = new ClanarinaDto
                {
                    Id = clanarina.Id,
                    DatumPocetka = clanarina.DatumPocetka,
                    DatumIsteka = clanarina.DatumIsteka
                };
            }

            ViewBag.JeMojProfil = false;
            ViewBag.ProfilKorisnikId = id;

            var uloga = korisnik.Uloga?.Naziv ?? string.Empty;
            if (string.Equals(uloga, RoleNames.Bibliotekar, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.NazadController = "Korisnik";
                ViewBag.NazadAction = "IndexBibliotekar";
                ViewBag.NazadLabel = "← Nazad na bibliotekare";
            }
            else
            {
                ViewBag.NazadController = "Korisnik";
                ViewBag.NazadAction = "Index";
                ViewBag.NazadLabel = "← Nazad na članove";
            }

            return View("Profil", korisnik);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new KorisnikCreateDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(KorisnikCreateDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Lozinka != model.PotvrdaLozinke)
            {
                ModelState.AddModelError(nameof(model.PotvrdaLozinke), "Lozinka i potvrda lozinke se ne poklapaju.");
                return View(model);
            }

            var existingUser = await _korisnikRepository.GetByEmailAsync(model.Email);
            if (existingUser is not null)
            {
                ModelState.AddModelError(nameof(model.Email), "Ta email adresa je već registrovana.");
                return View(model);
            }

            var korisnik = new SmartLib.Core.Models.Korisnik
            {
                Ime = model.Ime.Trim(),
                Prezime = model.Prezime.Trim(),
                Email = model.Email.Trim().ToLowerInvariant(),
                LozinkaHash = SmartLib.Infrastructure.Security.PasswordHasher.HashPassword(model.Lozinka),
                UlogaId = 1,
                Status = "aktivan",
                DatumKreiranja = DateTime.UtcNow
            };

            await _korisnikRepository.CreateAsync(korisnik);

            TempData["SuccessMessage"] = "Nalog člana je uspješno kreiran.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = RoleNames.Administrator)]
        public IActionResult CreateBibliotekar()
        {
            return View(new KorisnikCreateDto());
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<IActionResult> CreateBibliotekar(KorisnikCreateDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Lozinka != model.PotvrdaLozinke)
            {
                ModelState.AddModelError(nameof(model.PotvrdaLozinke), "Lozinka i potvrda lozinke se ne poklapaju.");
                return View(model);
            }

            var existingUser = await _korisnikRepository.GetByEmailAsync(model.Email);
            if (existingUser is not null)
            {
                ModelState.AddModelError(nameof(model.Email), "Ta email adresa je već registrovana.");
                return View(model);
            }

            var sviKorisnici = await _korisnikRepository.GetAllAsync();
            var bibliotekarUlogaId = sviKorisnici
                .FirstOrDefault(k => string.Equals(k.Uloga?.Naziv, RoleNames.Bibliotekar, StringComparison.OrdinalIgnoreCase))
                ?.UlogaId;

            if (bibliotekarUlogaId == null)
            {
                ModelState.AddModelError(string.Empty, "Uloga Bibliotekar nije pronađena u sistemu.");
                return View(model);
            }

            var korisnik = new Korisnik
            {
                Ime = model.Ime.Trim(),
                Prezime = model.Prezime.Trim(),
                Email = model.Email.Trim().ToLowerInvariant(),
                LozinkaHash = SmartLib.Infrastructure.Security.PasswordHasher.HashPassword(model.Lozinka),
                UlogaId = bibliotekarUlogaId.Value,
                Status = "aktivan",
                DatumKreiranja = DateTime.UtcNow
            };

            await _korisnikRepository.CreateAsync(korisnik);

            TempData["SuccessMessage"] = "Nalog bibliotekara je uspješno kreiran.";
            return RedirectToAction(nameof(IndexBibliotekar));
        }

        [HttpPost]
        public async Task<IActionResult> Deaktiviraj(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik is null)
                return NotFound();

            // ── Zaštita: admin se ne može deaktivirati ──
            if (string.Equals(korisnik.Uloga?.Naziv, RoleNames.Administrator, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Administrator ne može biti deaktiviran.";
                return RedirectToAction(nameof(ProfilClana), new { id });
            }

            korisnik.Status = "deaktiviran";
            korisnik.DatumDeaktivacije = DateTime.UtcNow;
            await _korisnikRepository.UpdateAsync(korisnik);

            TempData["SuccessMessage"] = "Nalog člana je deaktiviran.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Aktiviraj(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik is null)
                return NotFound();

            var targetIsAdmin = string.Equals(
                korisnik.Uloga?.Naziv,
                RoleNames.Administrator,
                StringComparison.OrdinalIgnoreCase);

            if (targetIsAdmin && !User.IsInRole(RoleNames.Administrator))
            {
                TempData["ErrorMessage"] = "Samo administrator može reaktivirati admin nalog.";
                return RedirectToAction(nameof(ProfilClana), new { id });
            }

            korisnik.Status = "aktivan";
            korisnik.DatumDeaktivacije = null;
            await _korisnikRepository.UpdateAsync(korisnik);

            TempData["SuccessMessage"] = "Nalog je reaktiviran.";
            return RedirectToAction(nameof(ProfilClana), new { id });
        }

        // ─── IZMIJENJENO: dohvat UlogaId + lista uloga za dropdown ──────────────
        [HttpGet]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<IActionResult> Uredi(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik == null)
                return NotFound();

            var jeAdmin = string.Equals(
                korisnik.Uloga?.Naziv, RoleNames.Administrator, StringComparison.OrdinalIgnoreCase);

            // Dohvati distinktne uloge iz baze (preko korisnika koji ih imaju)
            var sviKorisnici = await _korisnikRepository.GetAllAsync();
            var uloge = sviKorisnici
                .Where(k => k.Uloga != null)
                .Select(k => new { k.UlogaId, Naziv = k.Uloga!.Naziv })
                .DistinctBy(u => u.UlogaId)
                .OrderBy(u => u.UlogaId)
                .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = u.UlogaId.ToString(),
                    Text = u.Naziv,
                    Selected = (u.UlogaId == korisnik.UlogaId)
                })
                .ToList();

            ViewBag.Uloge = uloge;
            ViewBag.JeAdmin = jeAdmin;
            ViewBag.TrenutnaUloga = korisnik.Uloga?.Naziv ?? "—";
            ViewBag.TrenutnoIme = $"{korisnik.Ime} {korisnik.Prezime}";

            var dto = new UrediKorisnikaDto
            {
                Id = korisnik.Id,
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Status = korisnik.Status,
                UlogaId = korisnik.UlogaId
            };

            return View(dto);
        }

        // ─── IZMIJENJENO: primjena uloge + zaštita admina ────────────────────────
        [HttpPost]
        [Authorize(Roles = RoleNames.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(int id, UrediKorisnikaDto model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik == null)
                return NotFound();

            var jeAdmin = string.Equals(
                korisnik.Uloga?.Naziv, RoleNames.Administrator, StringComparison.OrdinalIgnoreCase);

            korisnik.Ime = model.Ime.Trim();
            korisnik.Prezime = model.Prezime.Trim();

            if (!jeAdmin)
            {
                // Samo ne-admin korisnicima možemo mijenjati ulogu i status
                korisnik.UlogaId = model.UlogaId;
                if (!string.Equals(korisnik.Status, model.Status, StringComparison.OrdinalIgnoreCase))
                {
                    korisnik.Status = model.Status;
                    if (string.Equals(model.Status, "deaktiviran", StringComparison.OrdinalIgnoreCase))
                        korisnik.DatumDeaktivacije = DateTime.UtcNow;
                    else if (string.Equals(model.Status, "aktivan", StringComparison.OrdinalIgnoreCase))
                        korisnik.DatumDeaktivacije = null;
                }
            }
            // Ako je admin — uloga i status ostaju nepromijenjeni

            if (!string.IsNullOrWhiteSpace(model.NovaLozinka))
                korisnik.LozinkaHash = SmartLib.Infrastructure.Security.PasswordHasher.HashPassword(model.NovaLozinka);

            await _korisnikRepository.UpdateAsync(korisnik);

            TempData["SuccessMessage"] = "Profil je uspješno ažuriran.";
            return RedirectToAction(nameof(ProfilClana), new { id });
        }
    }
}