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
        private readonly IClanarinaRepository _clanarinaRepository; // NOVO

        public KorisnikController(
            IKorisnikRepository korisnikRepository,
            IClanarinaRepository clanarinaRepository) // NOVO
        {
            _korisnikRepository = korisnikRepository;
            _clanarinaRepository = clanarinaRepository; // NOVO
        }

        public async Task<IActionResult> Index()
        {
            var korisnici = await _korisnikRepository.GetAllAsync();
            var clanovi = korisnici
                .Where(k => string.Equals(k.Uloga?.Naziv, RoleNames.Clan, StringComparison.OrdinalIgnoreCase))
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .Select(k => new KorisnikDto
                {
                    Id = k.Id,
                    Ime = k.Ime,
                    Prezime = k.Prezime,
                    Email = k.Email,
                    Uloga = k.Uloga?.Naziv,
                    Status = k.Status,
                    DatumKreiranja = k.DatumKreiranja
                })
                .ToList();

            return View(clanovi);
        }

        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<IActionResult> IndexBibliotekar()
        {
            var korisnici = await _korisnikRepository.GetAllAsync();
            var bibliotekari = korisnici
                .Where(k => string.Equals(k.Uloga?.Naziv, RoleNames.Bibliotekar, StringComparison.OrdinalIgnoreCase))
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .Select(k => new KorisnikDto
                {
                    Id = k.Id,
                    Ime = k.Ime,
                    Prezime = k.Prezime,
                    Email = k.Email,
                    Uloga = k.Uloga?.Naziv,
                    Status = k.Status,
                    DatumKreiranja = k.DatumKreiranja
                })
                .ToList();

            return View(bibliotekari);
        }

        // ─── IZMIJENJENO: dohvat vlastite članarine ──────────────────────────
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

            // Dohvati članarinu i proslijedi u ViewBag
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

        // ─── IZMIJENJENO: dohvat članarine za profil člana ───────────────────
        [HttpGet]
        public async Task<IActionResult> ProfilClana(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik == null)
                return NotFound();

            // Dohvati članarinu i proslijedi u ViewBag
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

            korisnik.Status = "deaktiviran";
            await _korisnikRepository.UpdateAsync(korisnik);

            TempData["SuccessMessage"] = "Nalog člana je deaktiviran.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<IActionResult> Uredi(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik == null)
                return NotFound();

            var dto = new UrediKorisnikaDto
            {
                Id = korisnik.Id,
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Status = korisnik.Status
            };

            return View(dto);
        }

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

            korisnik.Ime = model.Ime.Trim();
            korisnik.Prezime = model.Prezime.Trim();
            korisnik.Status = model.Status;

            if (!string.IsNullOrWhiteSpace(model.NovaLozinka))
                korisnik.LozinkaHash = SmartLib.Infrastructure.Security.PasswordHasher.HashPassword(model.NovaLozinka);

            await _korisnikRepository.UpdateAsync(korisnik);

            TempData["SuccessMessage"] = "Profil je uspješno ažuriran.";
            return RedirectToAction(nameof(ProfilClana), new { id });
        }
    }
}