using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using System.Security.Claims;
using System.Globalization;
using System.Text;

namespace SmartLib.Web.Controllers
{
    [Authorize]
    public class RecenzijaController : Controller
    {
        private readonly IRecenzijaRepository _recenzijaRepo;
        private readonly IKnjigaRepository _knjigaRepo;
        private readonly IZaduzenjeRepository _zaduzenjeRepo;

        public RecenzijaController(
            IRecenzijaRepository recenzijaRepo, 
            IKnjigaRepository knjigaRepo,
            IZaduzenjeRepository zaduzenjeRepo)
        {
            _recenzijaRepo = recenzijaRepo;
            _knjigaRepo = knjigaRepo;
            _zaduzenjeRepo = zaduzenjeRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Dodaj(int knjigaId)
        {
            var uId = GetUserId();
            if (!uId.HasValue) return Challenge();

            // Može recenzirati samo ako je vraćena (ili barem zaduživana)
            var history = await _zaduzenjeRepo.GetHistoryByKorisnikAsync(uId.Value);
            bool hasRead = history.Any(z => z.Primjerak?.KnjigaId == knjigaId);

            if (!hasRead)
            {
                TempData["ErrorMessage"] = "Knjigu možete ocijeniti tek nakon što je pročitate (vratite).";
                return RedirectToAction("MojaHistorija", "Zaduzenje");
            }

            var reviewed = await _recenzijaRepo.HasUserReviewedAsync(knjigaId, uId.Value);
            if (reviewed)
            {
                TempData["ReviewErrorMessage"] = "Već ste ostavili recenziju za ovu knjigu.";
                return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
            }

            var knjiga = await _knjigaRepo.GetByIdAsync(knjigaId);
            if (knjiga == null) return NotFound();

            ViewBag.KnjigaNaslov = knjiga.Naslov;
            ViewBag.KnjigaId = knjigaId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(int KnjigaId, int Ocjena, string? Komentar)
        {
            var uId = GetUserId();
            if (!uId.HasValue) return Challenge();

            if (Ocjena < 1 || Ocjena > 5)
            {
                ModelState.AddModelError(nameof(Ocjena), "Ocjena mora biti između 1 i 5.");
            }

            if (!string.IsNullOrWhiteSpace(Komentar) && ContainsBlockedWords(Komentar))
            {
                ModelState.AddModelError(nameof(Komentar), "Recenzija sadrži neprimjeren jezik. Molimo prilagodite sadržaj.");
            }

            var reviewed = await _recenzijaRepo.HasUserReviewedAsync(KnjigaId, uId.Value);
            if (reviewed)
            {
                TempData["ReviewErrorMessage"] = "Već ste ostavili recenziju za ovu knjigu.";
                return RedirectToAction("Details", "Knjiga", new { id = KnjigaId });
            }

            if (!ModelState.IsValid)
            {
                var knjiga = await _knjigaRepo.GetByIdAsync(KnjigaId);
                if (knjiga == null) return NotFound();

                ViewBag.KnjigaNaslov = knjiga.Naslov;
                ViewBag.KnjigaId = KnjigaId;
                ViewBag.PrefillOcjena = Ocjena;
                ViewBag.PrefillKomentar = Komentar ?? string.Empty;
                TempData["ErrorMessage"] = "Recenzija nije sačuvana. Provjerite unesene podatke.";
                return View();
            }

            var r = new Recenzija
            {
                KnjigaId = KnjigaId,
                KorisnikId = uId.Value,
                Ocjena = Ocjena,
                Komentar = Komentar ?? string.Empty,
                DatumKreiranja = DateTime.UtcNow
            };

            await _recenzijaRepo.AddAsync(r);

            TempData["ReviewSuccessMessage"] = "Uspješno ste ostavili recenziju!";
            return RedirectToAction("Details", "Knjiga", new { id = KnjigaId });
        }

        [HttpPost]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int knjigaId)
        {
            var deleted = await _recenzijaRepo.DeleteAsync(id);
            TempData[deleted ? "SuccessMessage" : "ErrorMessage"] = deleted
                ? "Recenzija je uklonjena."
                : "Recenziju nije moguće ukloniti.";

            return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
        }

        private int? GetUserId()
        {
            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idValue, out var id)) return id;
            return null;
        }

        private static readonly string[] BlockedTerms =
        {
            "jeb", "kur", "pizd", "picka", "sranje", "fuck", "shit", "asshole", "bitch", "bastard"
        };

        private static bool ContainsBlockedWords(string input)
        {
            var normalized = NormalizeForFilter(input);
            return BlockedTerms.Any(term => normalized.Contains(term));
        }

        private static string NormalizeForFilter(string input)
        {
            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);

            foreach (var ch in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (category == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                sb.Append(char.ToLowerInvariant(ch));
            }

            return sb.ToString();
        }
    }
}
