using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Services;
using System.Security.Claims;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Rezervacije modul — Kreiranje i otkazivanje rezervacija (MVC)
    /// US-69, US-70, US-71, US-72, US-73, US-79, US-80
    /// </summary>
    [Authorize]
    public class RezervacijaController : Controller
    {
        private readonly IRezervacijaRepository _rezervacijaRepo;
        private readonly IKnjigaRepository _knjigaRepo;
        private readonly BibliotekariNotifikacijaService _bibliotekariNotifikacija;
        private readonly IZaduzenjeRepository _zaduzenjeRepo;

        public RezervacijaController(
            IRezervacijaRepository rezervacijaRepo,
            IKnjigaRepository knjigaRepo,
            IZaduzenjeRepository zaduzenjeRepo,
            BibliotekariNotifikacijaService bibliotekariNotifikacija)
        {
            _rezervacijaRepo = rezervacijaRepo;
            _knjigaRepo = knjigaRepo;
            _zaduzenjeRepo = zaduzenjeRepo;
            _bibliotekariNotifikacija = bibliotekariNotifikacija;
        }

        // US-73: Pregled svih aktivnih rezervacija (bibliotekar/administrator)
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        public async Task<IActionResult> Index(string? filter = null)
        {
            var sve = await _rezervacijaRepo.GetActiveAsync();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var f = filter.Trim().ToLowerInvariant();
                sve = sve.Where(r =>
                    ($"{r.Korisnik?.Ime} {r.Korisnik?.Prezime}").ToLowerInvariant().Contains(f) ||
                    (r.Korisnik?.Email ?? string.Empty).ToLowerInvariant().Contains(f) ||
                    (r.Knjiga?.Naslov ?? string.Empty).ToLowerInvariant().Contains(f));
            }

            var model = new AktivneRezervacijeViewModel
            {
                Filter = filter,
                Rezervacije = sve.Select(MapToViewModel).ToList()
            };

            return View(model);
        }

        // US-71: Pregled vlastitih aktivnih rezervacija (prijavljeni član)
        public async Task<IActionResult> Moje()
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return RedirectToAction("Login", "Auth");

            var rezervacije = await _rezervacijaRepo.GetByKorisnikAsync(korisnikId);
            var model = rezervacije.Select(MapToViewModel).ToList();
            return View(model);
        }

        // US-69, US-70, US-79: Kreiranje rezervacije
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int knjigaId)
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return RedirectToAction("Login", "Auth");

            var knjiga = await _knjigaRepo.GetByIdAsync(knjigaId);
            if (knjiga == null)
            {
                TempData["ErrorMessage"] = "Knjiga nije pronađena.";
                return RedirectToAction("Index", "Knjiga");
            }

            // US-69: Rezervacija dozvoljena samo kad nema dostupnih primjeraka
            bool imaDostupnih = knjiga.Primjerci.Any(p => p.Status == "dostupan");
            if (imaDostupnih)
            {
                TempData["ErrorMessage"] = "Rezervacija nije moguća — knjiga ima dostupnih primjeraka.";
                return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
            }

            // US-69: Jedan član ne može imati dvije aktivne rezervacije iste knjige
            bool imaDuplikat = await _rezervacijaRepo.HasActiveAsync(korisnikId, knjigaId);
            if (imaDuplikat)
            {
                TempData["ErrorMessage"] = "Već imate aktivnu rezervaciju za ovu knjigu.";
                return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
            }

            bool imaKasnela = await _zaduzenjeRepo.ImaKasnelaZaduzenjaAsync(korisnikId);
            if (imaKasnela)
            {
                TempData["ErrorMessage"] =
                    "Nije moguće kreirati rezervaciju — imate jedno ili više zakasnjelih zaduženja koja nisu vraćena. Molimo vas da kontaktirate biblioteku.";
                return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
            }

            var rezervacija = new Rezervacija
            {
                KorisnikId = korisnikId,
                KnjigaId = knjigaId,
                DatumRezervacije = DateTime.UtcNow,
                DatumIsteka = DateTime.UtcNow.AddDays(7),
                Status = "aktivna"
            };

            await _rezervacijaRepo.CreateAsync(rezervacija);

            var rezervacijaZaEmail = await _rezervacijaRepo.GetByIdAsync(rezervacija.Id);
            if (rezervacijaZaEmail != null)
            {
                try
                {
                    await _bibliotekariNotifikacija.ObavijestiBibliotekareNovaRezervacijaAsync(rezervacijaZaEmail);
                }
                catch { /* greška u emailu ne smije blokirati akciju */ }
            }

            TempData["SuccessMessage"] = "Rezervacija je uspješno kreirana. Knjiga čeka vas 7 dana.";
            return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
        }

        // US-72, US-80: Otkazivanje rezervacije
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Otkazi(int id)
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return RedirectToAction("Login", "Auth");

            var rezervacija = await _rezervacijaRepo.GetByIdAsync(id);
            if (rezervacija == null)
            {
                TempData["ErrorMessage"] = "Rezervacija nije pronađena.";
                return RedirectToAction(nameof(Moje));
            }

            // Samo vlasnik može otkazati svoju rezervaciju
            if (rezervacija.KorisnikId != korisnikId)
            {
                TempData["ErrorMessage"] = "Nemate pravo otkazati ovu rezervaciju.";
                return RedirectToAction(nameof(Moje));
            }

            if (rezervacija.Status != "aktivna")
            {
                TempData["ErrorMessage"] = "Rezervacija nije aktivna.";
                return RedirectToAction(nameof(Moje));
            }

            rezervacija.Status = "otkazana";
            await _rezervacijaRepo.UpdateAsync(rezervacija);
            TempData["SuccessMessage"] = "Rezervacija je uspješno otkazana.";
            return RedirectToAction(nameof(Moje));
        }

        private static RezervacijaViewModel MapToViewModel(Rezervacija r) => new()
        {
            Id = r.Id,
            KnjigaNaslov = r.Knjiga?.Naslov ?? "-",
            KorisnikIme = r.Korisnik != null ? $"{r.Korisnik.Ime} {r.Korisnik.Prezime}" : "-",
            KorisnikEmail = r.Korisnik?.Email ?? "-",
            DatumRezervacije = r.DatumRezervacije,
            DatumIsteka = r.DatumIsteka,
            Status = r.Status,
            KnjigaDostupna = r.Knjiga?.Primjerci.Any(p => p.Status == "dostupan") ?? false
        };
    }
}
