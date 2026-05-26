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
        private readonly IRecenzijaPrijavaRepository _prijavaRepo;
        private readonly INotifikacijaRepository _notifikacijaRepo;
        private readonly IKorisnikRepository _korisnikRepo;

        public RecenzijaController(
            IRecenzijaRepository recenzijaRepo, 
            IKnjigaRepository knjigaRepo,
            IZaduzenjeRepository zaduzenjeRepo,
            IRecenzijaPrijavaRepository prijavaRepo,
            INotifikacijaRepository notifikacijaRepo,
            IKorisnikRepository korisnikRepo)
        {
            _recenzijaRepo = recenzijaRepo;
            _knjigaRepo = knjigaRepo;
            _zaduzenjeRepo = zaduzenjeRepo;
            _prijavaRepo = prijavaRepo;
            _notifikacijaRepo = notifikacijaRepo;
            _korisnikRepo = korisnikRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Dodaj(int knjigaId)
        {
            var uId = GetUserId();
            if (!uId.HasValue) return Challenge();

            var user = await _korisnikRepo.GetByIdAsync(uId.Value);
            if (user?.DatumZabraneDo > DateTime.UtcNow)
            {
                TempData["ReviewErrorMessage"] = $"Imate aktivnu zabranu i ne možete ostavljati recenzije do {user.DatumZabraneDo.Value.ToLocalTime():dd.MM.yyyy HH:mm}.";
                return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
            }

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

            var user = await _korisnikRepo.GetByIdAsync(uId.Value);
            if (user?.DatumZabraneDo > DateTime.UtcNow)
            {
                TempData["ReviewErrorMessage"] = $"Imate aktivnu zabranu i ne možete ostavljati recenzije do {user.DatumZabraneDo.Value.ToLocalTime():dd.MM.yyyy HH:mm}.";
                return RedirectToAction("Details", "Knjiga", new { id = KnjigaId });
            }

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
            var recenzija = await _recenzijaRepo.GetByIdAsync(id);
            int? authorId = recenzija?.KorisnikId;

            var deleted = await _recenzijaRepo.DeleteAsync(id);
            TempData[deleted ? "SuccessMessage" : "ErrorMessage"] = deleted
                ? "Recenzija je uklonjena."
                : "Recenziju nije moguće ukloniti.";

            if (deleted && authorId.HasValue)
            {
                var author = await _korisnikRepo.GetByIdAsync(authorId.Value);
                if (author != null)
                {
                    author.BrojUklonjenihSadrzaja++;
                    if (author.BrojUklonjenihSadrzaja >= 3)
                    {
                        author.DatumZabraneDo = DateTime.UtcNow.AddDays(7);
                        author.BrojUklonjenihSadrzaja = 0;
                    }
                    await _korisnikRepo.UpdateAsync(author);
                }
            }

            return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Prijavi(int recenzijaId, int knjigaId, string? razlog)
        {
            var uId = GetUserId();
            if (!uId.HasValue) return Challenge();

            var recenzija = await _recenzijaRepo.GetByIdAsync(recenzijaId);
            if (recenzija == null) return NotFound();

            if (await _prijavaRepo.HasUserReportedAsync(recenzijaId, uId.Value))
            {
                TempData["ReviewErrorMessage"] = "Već ste prijavili ovu recenziju.";
                return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
            }

            var prijava = new RecenzijaPrijava
            {
                RecenzijaId = recenzijaId,
                PrijavioKorisnikId = uId.Value,
                Razlog = string.IsNullOrWhiteSpace(razlog) ? null : razlog.Trim(),
                DatumKreiranja = DateTime.UtcNow
            };

            await _prijavaRepo.AddAsync(prijava);

            var staff = (await _korisnikRepo.GetAllAsync())
                .Where(k => string.Equals(k.Uloga?.Naziv, RoleNames.Administrator, StringComparison.OrdinalIgnoreCase)
                         || string.Equals(k.Uloga?.Naziv, RoleNames.Bibliotekar, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var naslov = recenzija.Knjiga?.Naslov ?? "Knjiga";
            var prijavaUrl = Url.Action(nameof(Prijave), "Recenzija") ?? "/Recenzija/Prijave";
            var notifList = staff.Select(s => new Notifikacija
            {
                KorisnikId = s.Id,
                Naslov = "Prijavljena recenzija",
                Poruka = $"Prijavljena je recenzija za knjigu: {naslov}.",
                Tip = "Recenzija",
                LinkUrl = prijavaUrl,
                DatumKreiranja = DateTime.UtcNow
            });

            await _notifikacijaRepo.AddRangeAsync(notifList);

            TempData["ReviewSuccessMessage"] = "Recenzija je prijavljena. Hvala na prijavi.";
            return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
        }

        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<IActionResult> Prijave()
        {
            var prijave = await _prijavaRepo.GetOpenAsync();
            return View(prijave);
        }

        [HttpPost]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Razrijesi(int prijavaId)
        {
            var uId = GetUserId();
            if (!uId.HasValue) return Challenge();

            var resolved = await _prijavaRepo.TryResolveAsync(prijavaId, uId.Value);
            var prijava = await _prijavaRepo.GetByIdAsync(prijavaId);

            if (!resolved)
            {
                var resolverName = prijava?.RazrijesioKorisnik != null
                    ? $"{prijava.RazrijesioKorisnik.Ime} {prijava.RazrijesioKorisnik.Prezime}"
                    : "drugi korisnik";

                await _notifikacijaRepo.AddAsync(new Notifikacija
                {
                    KorisnikId = uId.Value,
                    Naslov = "Prijava već razriješena",
                    Poruka = $"Prijava je već razriješena od: {resolverName}.",
                    Tip = "Recenzija",
                    LinkUrl = Url.Action(nameof(Prijave), "Recenzija") ?? "/Recenzija/Prijave",
                    DatumKreiranja = DateTime.UtcNow
                });

                TempData["ErrorMessage"] = "Prijava je već razriješena od drugog korisnika.";
                return RedirectToAction(nameof(Prijave));
            }

            var staff = (await _korisnikRepo.GetAllAsync())
                .Where(k => string.Equals(k.Uloga?.Naziv, RoleNames.Administrator, StringComparison.OrdinalIgnoreCase)
                         || string.Equals(k.Uloga?.Naziv, RoleNames.Bibliotekar, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var resolver = staff.FirstOrDefault(k => k.Id == uId.Value);
            var resolverDisplay = resolver != null ? $"{resolver.Ime} {resolver.Prezime}" : "osoblje";
            var knjigaNaslov = prijava?.Recenzija?.Knjiga?.Naslov ?? "Knjiga";
            var notifLink = Url.Action(nameof(Prijave), "Recenzija") ?? "/Recenzija/Prijave";

            var notifList = staff.Where(s => s.Id != uId.Value).Select(s => new Notifikacija
            {
                KorisnikId = s.Id,
                Naslov = "Prijava recenzije razriješena",
                Poruka = $"Prijava recenzije za knjigu {knjigaNaslov} je razriješena od: {resolverDisplay}.",
                Tip = "Recenzija",
                LinkUrl = notifLink,
                DatumKreiranja = DateTime.UtcNow
            });

            await _notifikacijaRepo.AddRangeAsync(notifList);

            TempData["SuccessMessage"] = "Prijava je razriješena.";
            return RedirectToAction(nameof(Prijave));
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
