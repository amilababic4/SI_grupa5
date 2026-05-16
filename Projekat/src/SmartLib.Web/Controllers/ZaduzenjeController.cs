using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.Web.Controllers
{
    [Authorize]
    public class ZaduzenjeController : Controller
    {
        private readonly IZaduzenjeRepository _zaduzenjeRepo;
        private readonly IKorisnikRepository _korisnikRepo;
        private readonly IKnjigaRepository _knjigaRepo;
        private readonly IPrimjerakRepository _primjerakRepo;

        public ZaduzenjeController(
            IZaduzenjeRepository zaduzenjeRepo,
            IKorisnikRepository korisnikRepo,
            IKnjigaRepository knjigaRepo,
            IPrimjerakRepository primjerakRepo)
        {
            _zaduzenjeRepo = zaduzenjeRepo;
            _korisnikRepo = korisnikRepo;
            _knjigaRepo = knjigaRepo;
            _primjerakRepo = primjerakRepo;
        }

        // US-65, US-66, US-68: Aktivna zaduženja (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        public async Task<IActionResult> Index(string? clan)
        {
            var sva = await _zaduzenjeRepo.GetActiveAsync();

            if (!string.IsNullOrWhiteSpace(clan))
            {
                var filter = clan.Trim().ToLowerInvariant();
                sva = sva.Where(z =>
                    ($"{z.Korisnik?.Ime} {z.Korisnik?.Prezime}").ToLowerInvariant().Contains(filter) ||
                    (z.Korisnik?.Email ?? string.Empty).ToLowerInvariant().Contains(filter));
            }

            var model = new AktivnaZaduzenjaViewModel
            {
                Clan = clan,
                Zaduzenja = sva.Select(MapToViewModel).ToList()
            };

            return View(model);
        }

        // US-62, US-63, US-64: Vlastita zaduženja (prijavljeni korisnik)
        public async Task<IActionResult> Moja()
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return RedirectToAction("Login", "Auth");

            var zaduzenja = await _zaduzenjeRepo.GetByKorisnikAsync(korisnikId);
            var model = zaduzenja.Select(MapToViewModel).ToList();
            return View(model);
        }

        // US-67: Detalji zaduženja (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        public async Task<IActionResult> Details(int id)
        {
            var z = await _zaduzenjeRepo.GetByIdAsync(id);
            if (z == null) return NotFound();
            return View(MapToViewModel(z));
        }

        // US-44: Forma za novo zaduživanje (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateCreateDropdowns();
            return View(new ZaduzenjeCreateDto());
        }

        // US-44, US-46, US-47: Kreiranje zaduženja (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Zaduzi(ZaduzenjeCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCreateDropdowns(model);
                return View("Create", model);
            }

            // US-47: Provjera dostupnosti primjerka
            var primjerak = await _primjerakRepo.GetByIdAsync(model.PrimjerakId);
            if (primjerak == null || primjerak.Status != "dostupan")
            {
                ModelState.AddModelError(string.Empty, "Odabrani primjerak nije dostupan za zaduživanje.");
                await PopulateCreateDropdowns(model);
                return View("Create", model);
            }

            // US-47: Provjera duplikata aktivnog zaduženja za isti primjerak
            var imaDuplikat = await _primjerakRepo.HasActiveZaduzenjeAsync(model.PrimjerakId);
            if (imaDuplikat)
            {
                ModelState.AddModelError(string.Empty, "Odabrani primjerak već ima aktivno zaduženje.");
                await PopulateCreateDropdowns(model);
                return View("Create", model);
            }

            // Provjera da rok vraćanja nije u prošlosti
            if (model.DatumPovratka.HasValue && model.DatumPovratka.Value.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(model.DatumPovratka), "Datum povratka ne može biti u prošlosti.");
                await PopulateCreateDropdowns(model);
                return View("Create", model);
            }

            // Rok vraćanja: uneseni datum ili automatski 2 mjeseca
            var datumZaduzivanja = DateTime.UtcNow;
            var datumPlaniranogVracanja = model.DatumPovratka.HasValue
                ? DateTime.SpecifyKind(model.DatumPovratka.Value.Date, DateTimeKind.Utc)
                : datumZaduzivanja.AddMonths(2);

            var zaduzenje = new Zaduzenje
            {
                KorisnikId = model.KorisnikId,
                PrimjerakId = model.PrimjerakId,
                DatumZaduzivanja = datumZaduzivanja,
                DatumPlaniranogVracanja = datumPlaniranogVracanja,
                Status = "aktivno"
            };

            await _zaduzenjeRepo.CreateAsync(zaduzenje);
            await _primjerakRepo.UpdateStatusAsync(model.PrimjerakId, "zadužen");

            TempData["SuccessMessage"] = "Zaduživanje je uspješno evidentirano.";
            return RedirectToAction(nameof(Index));
        }

        // US-45: Stranica za potvrdu vraćanja (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpGet]
        public async Task<IActionResult> VratiPotvrda(int id)
        {
            var z = await _zaduzenjeRepo.GetByIdAsync(id);
            if (z == null) return NotFound();
            if (z.Status != "aktivno") return RedirectToAction(nameof(Index));
            return View(MapToViewModel(z));
        }

        // US-45: Evidencija vraćanja knjige (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vrati(int id)
        {
            var z = await _zaduzenjeRepo.GetByIdAsync(id);
            if (z == null) return NotFound();

            if (z.Status != "aktivno")
            {
                TempData["ErrorMessage"] = "Zaduženje nije aktivno.";
                return RedirectToAction(nameof(Index));
            }

            z.Status = "zatvoreno";
            z.DatumStvarnogVracanja = DateTime.UtcNow;
            await _zaduzenjeRepo.UpdateAsync(z);
            await _primjerakRepo.UpdateStatusAsync(z.PrimjerakId, "dostupan");

            TempData["SuccessMessage"] = "Vraćanje knjige je uspješno evidentirano.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // US-54: Evidencija vraćanja knjige (bibliotekar)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        public async Task<IActionResult> Historija(string? clan)
        {
            var granica = DateTime.UtcNow.AddYears(-3);
            var sva = await _zaduzenjeRepo.GetClosedSinceAsync(granica);

            if (!string.IsNullOrWhiteSpace(clan))
            {
                var filter = clan.Trim().ToLowerInvariant();
                sva = sva.Where(z =>
                    ($"{z.Korisnik?.Ime} {z.Korisnik?.Prezime}").ToLowerInvariant().Contains(filter) ||
                    (z.Korisnik?.Email ?? string.Empty).ToLowerInvariant().Contains(filter));
            }

            var model = new HistorijaZaduzenjaViewModel
            {
                Clan = clan,
                Zaduzenja = sva.Select(MapToViewModel).ToList()
            };

            return View(model);
        }

        // Nema user story, dodao jer mi je bilo logično 
        [Authorize]
        public async Task<IActionResult> MojaHistorija()
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return RedirectToAction("Login", "Auth");

            var granica = DateTime.UtcNow.AddYears(-3);

            var mojaZaduzenja = await _zaduzenjeRepo.GetClosedHistoryForKorisnikAsync(korisnikId, granica);

            var model = mojaZaduzenja.Select(MapToViewModel).ToList();

            return View(model);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static ZaduzenjeViewModel MapToViewModel(Zaduzenje z)
        {
            var now = DateTime.UtcNow;
            var zakasnilo = z.DatumPlaniranogVracanja < now;
            return new ZaduzenjeViewModel
            {
                Id = z.Id,
                KorisnikIme = z.Korisnik != null ? $"{z.Korisnik.Ime} {z.Korisnik.Prezime}" : "-",
                KorisnikEmail = z.Korisnik?.Email ?? "-",
                KnjigaNaslov = z.Primjerak?.Knjiga?.Naslov ?? "-",
                InventarniBroj = z.Primjerak?.InventarniBroj ?? "-",
                DatumZaduzivanja = z.DatumZaduzivanja,
                DatumPlaniranogVracanja = z.DatumPlaniranogVracanja,
                DatumStvarnogVracanja = z.DatumStvarnogVracanja,
                Status = z.Status,
                JeZakasnilo = zakasnilo,
                RokSeBliži = !zakasnilo && z.DatumPlaniranogVracanja <= now.AddDays(3)
            };
        }

        private async Task PopulateCreateDropdowns(ZaduzenjeCreateDto? selected = null)
        {
            var sviKorisnici = await _korisnikRepo.GetAllAsync();
            var clanovi = sviKorisnici
                .Where(k => k.Uloga?.Naziv == RoleNames.Clan)
                .ToList();

            ViewBag.ClanDataJson = JsonSerializer.Serialize(
                clanovi.Select(k => new { id = k.Id, text = $"{k.Ime} {k.Prezime} ({k.Email})" }));

            var sveknjige = await _knjigaRepo.SearchAsync(null, null);
            var knjigeSaDostupnim = sveknjige
                .Where(k => k.Primjerci.Any(p => p.Status == "dostupan"))
                .ToList();

            ViewBag.KnjigaDataJson = JsonSerializer.Serialize(
                knjigeSaDostupnim.Select(k => new { id = k.Id, text = $"{k.Naslov} - {k.Autor}" }));

            ViewBag.PrimjerakDataJson = JsonSerializer.Serialize(
                knjigeSaDostupnim.SelectMany(k => k.Primjerci
                    .Where(p => p.Status == "dostupan")
                    .Select(p => new { id = p.Id, knjigaId = p.KnjigaId, text = p.InventarniBroj })));

            ViewBag.SelectedKorisnikId = selected?.KorisnikId ?? 0;
            ViewBag.SelectedKnjigaId = selected?.KnjigaId ?? 0;
            ViewBag.SelectedPrimjerakId = selected?.PrimjerakId ?? 0;
        }
    }
}
