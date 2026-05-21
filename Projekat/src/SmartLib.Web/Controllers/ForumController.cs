using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using System.Security.Claims;
using System.Globalization;
using System.Text;

namespace SmartLib.Web.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        private readonly IForumRepository _forumRepository;
        private readonly ILogger<ForumController> _logger;

        public ForumController(IForumRepository forumRepository, ILogger<ForumController> logger)
        {
            _forumRepository = forumRepository;
            _logger = logger;
        }

        // PB-59: Kategorije i pregled
        [HttpGet]
        public async Task<IActionResult> Index(string? kategorija)
        {
            var objave = await _forumRepository.GetAllAsync(kategorija);
            var uId = GetUserId();

            var dtos = new List<ForumObjavaDto>();
            foreach (var o in objave)
            {
                bool reacted = uId.HasValue && await _forumRepository.HasReakcijaAsync(o.Id, uId.Value);

                dtos.Add(new ForumObjavaDto
                {
                    Id = o.Id,
                    Naslov = o.Naslov,
                    Sadrzaj = o.Sadrzaj,
                    Kategorija = o.Kategorija,
                    DatumKreiranja = o.DatumKreiranja,
                    Zakljucana = o.Zakljucana,
                    AutorIme = o.Korisnik?.Ime + " " + o.Korisnik?.Prezime,
                    AutorUloga = o.Korisnik?.Uloga?.Naziv ?? "Član",
                    KorisnikId = o.KorisnikId,
                    BrojKomentara = o.Komentari.Count,
                    BrojReakcija = o.Reakcije.Count,
                    KorisnikJeReagovao = reacted
                });
            }

            var vm = new ForumIndexViewModel
            {
                Objave = dtos,
                AktivnaKategorija = kategorija,
                Kategorije = _forumRepository.GetKategorije().ToList()
            };

            return View(vm);
        }

        // PB-57: Pregled jedne objave (sa komentarima i reakcijama)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var o = await _forumRepository.GetByIdAsync(id);
            if (o == null) return NotFound();

            var uId = GetUserId();
            bool reacted = uId.HasValue && await _forumRepository.HasReakcijaAsync(o.Id, uId.Value);

            var dto = new ForumObjavaDto
            {
                Id = o.Id,
                Naslov = o.Naslov,
                Sadrzaj = o.Sadrzaj,
                Kategorija = o.Kategorija,
                DatumKreiranja = o.DatumKreiranja,
                Zakljucana = o.Zakljucana,
                AutorIme = o.Korisnik?.Ime + " " + o.Korisnik?.Prezime,
                AutorUloga = o.Korisnik?.Uloga?.Naziv ?? "Član",
                KorisnikId = o.KorisnikId,
                BrojKomentara = o.Komentari.Count,
                BrojReakcija = o.Reakcije.Count,
                KorisnikJeReagovao = reacted,
                Komentari = o.Komentari.OrderBy(k => k.DatumKreiranja).Select(k => new ForumKomentarDto
                {
                    Id = k.Id,
                    Sadrzaj = k.Sadrzaj,
                    DatumKreiranja = k.DatumKreiranja,
                    AutorIme = k.Korisnik?.Ime + " " + k.Korisnik?.Prezime,
                    AutorUloga = k.Korisnik?.Uloga?.Naziv ?? "Član",
                    KorisnikId = k.KorisnikId
                }).ToList()
            };

            return View(dto);
        }

        // PB-57: Forma za kreiranje
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Kategorije = _forumRepository.GetKategorije();
            return View(new ForumObjavaCreateDto());
        }

        // PB-57: Kreiranje objave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ForumObjavaCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Kategorije = _forumRepository.GetKategorije();
                return View(model);
            }

            var uId = GetUserId();
            if (!uId.HasValue) return Challenge();

            try
            {
                var objava = new ForumObjava
                {
                    Naslov = model.Naslov.Trim(),
                    Sadrzaj = model.Sadrzaj.Trim(),
                    Kategorija = model.Kategorija,
                    KorisnikId = uId.Value,
                    DatumKreiranja = DateTime.UtcNow
                };

                await _forumRepository.CreateAsync(objava);

                TempData["SuccessMessage"] = "Objava uspješno kreirana.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom kreiranja objave na forumu.");
                ModelState.AddModelError(string.Empty, "Došlo je do greške prilikom spašavanja objave u bazu: " + ex.Message);
                ViewBag.Kategorije = _forumRepository.GetKategorije();
                return View(model);
            }
        }

        // PB-58: Dodavanje komentara
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostComment(ForumKomentarCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Komentar ne može biti prazan.";
                return RedirectToAction(nameof(Details), new { id = model.ObjavaId });
            }

            if (ContainsBlockedWords(model.Sadrzaj ?? string.Empty))
            {
                TempData["ErrorMessage"] = "Komentar sadrži neprimjeren jezik. Molimo prilagodite sadržaj.";
                return RedirectToAction(nameof(Details), new { id = model.ObjavaId });
            }

            var uId = GetUserId();
            if (!uId.HasValue) return Challenge();

            var objava = await _forumRepository.GetByIdAsync(model.ObjavaId);
            if (objava == null) return NotFound();
            
            if (objava.Zakljucana)
            {
                TempData["ErrorMessage"] = "Ova diskusija je zaključana.";
                return RedirectToAction(nameof(Details), new { id = model.ObjavaId });
            }

            var k = new ForumKomentar
            {
                Sadrzaj = model.Sadrzaj.Trim(),
                ObjavaId = model.ObjavaId,
                KorisnikId = uId.Value,
                DatumKreiranja = DateTime.UtcNow
            };

            await _forumRepository.AddKomentarAsync(k);
            TempData["SuccessMessage"] = "Komentar uspješno dodan.";
            
            return RedirectToAction(nameof(Details), new { id = model.ObjavaId });
        }

        [HttpPost]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int komentarId, int objavaId)
        {
            var deleted = await _forumRepository.DeleteKomentarAsync(komentarId);
            TempData[deleted ? "SuccessMessage" : "ErrorMessage"] = deleted
                ? "Komentar je uklonjen."
                : "Komentar nije moguće ukloniti.";

            return RedirectToAction(nameof(Details), new { id = objavaId });
        }

        // PB-60: Reakcije na objave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleReaction(int objavaId)
        {
            var uId = GetUserId();
            if (!uId.HasValue) return Challenge();

            var added = await _forumRepository.ToggleReakcijaAsync(objavaId, uId.Value);

            // You could return JSON here if called via AJAX, but for simplicity let's redirect.
            if(Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var o = await _forumRepository.GetByIdAsync(objavaId);
                return Json(new { success = true, reacted = added, count = o?.Reakcije.Count ?? 0 });
            }

            return RedirectToAction(nameof(Details), new { id = objavaId });
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
