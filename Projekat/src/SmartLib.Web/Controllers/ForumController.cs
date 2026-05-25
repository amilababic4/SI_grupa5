using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using Microsoft.Extensions.Caching.Distributed;
using SmartLib.Infrastructure.Services;
using System.Security.Claims;
using System.Globalization;
using System.Net;
using System.Text;

namespace SmartLib.Web.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        private readonly IForumRepository _forumRepository;
        private readonly IKorisnikRepository _korisnikRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<ForumController> _logger;
        private readonly IDistributedCache _cache;
        private readonly CacheVersionStore _cacheVersions;
        private static readonly TimeSpan ForumIndexCacheTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan ForumDetailsCacheTtl = TimeSpan.FromMinutes(5);

        public ForumController(
            IForumRepository forumRepository,
            IKorisnikRepository korisnikRepository,
            IEmailService emailService,
            ILogger<ForumController> logger,
            IDistributedCache cache,
            CacheVersionStore cacheVersions)
        {
            _forumRepository = forumRepository;
            _korisnikRepository = korisnikRepository;
            _emailService = emailService;
            _logger = logger;
            _cache = cache;
            _cacheVersions = cacheVersions;
        }

        // PB-59: Kategorije i pregled
        [HttpGet]
        public async Task<IActionResult> Index(string? kategorija)
        {
            var uId = GetUserId();
            var userKey = uId?.ToString() ?? "anon";
            var categoryKey = string.IsNullOrWhiteSpace(kategorija) ? "all" : kategorija.Trim().ToLowerInvariant();
            var cacheKey = $"forum_index_v1_{_cacheVersions.ForumVersion}_{categoryKey}_{userKey}";

            var cached = await _cache.GetRecordAsync<ForumIndexViewModel>(cacheKey);
            if (cached != null)
                return View(cached);

            var objave = await _forumRepository.GetAllAsync(kategorija);

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

            await _cache.SetRecordAsync(cacheKey, vm, ForumIndexCacheTtl);

            return View(vm);
        }

        // PB-57: Pregled jedne objave (sa komentarima i reakcijama)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var uId = GetUserId();
            var userKey = uId?.ToString() ?? "anon";
            var cacheKey = $"forum_details_v1_{_cacheVersions.ForumVersion}_{id}_{userKey}";
            var cached = await _cache.GetRecordAsync<ForumObjavaDto>(cacheKey);
            if (cached != null)
                return View(cached);

            var o = await _forumRepository.GetByIdAsync(id);
            if (o == null) return NotFound();

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

            await _cache.SetRecordAsync(cacheKey, dto, ForumDetailsCacheTtl);

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
                _cacheVersions.BumpForumVersion();

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
            _cacheVersions.BumpForumVersion();
            
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

            if (deleted)
            {
                _cacheVersions.BumpForumVersion();
            }

            return RedirectToAction(nameof(Details), new { id = objavaId });
        }

        // PB-63: Prijava neadekvatnog komentara
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportComment(int komentarId, int objavaId, string? razlog)
        {
            var uId = GetUserId();
            if (!uId.HasValue) return Challenge();

            var objava = await _forumRepository.GetByIdAsync(objavaId);
            if (objava == null) return NotFound();

            var komentar = objava.Komentari.FirstOrDefault(k => k.Id == komentarId);
            if (komentar == null) return NotFound();

            if (await _forumRepository.HasKomentarPrijavaAsync(komentarId, uId.Value))
            {
                TempData["ErrorMessage"] = "Već ste prijavili ovaj komentar.";
                return RedirectToAction(nameof(Details), new { id = objavaId });
            }

            var prijava = new ForumKomentarPrijava
            {
                KomentarId = komentarId,
                PrijavioKorisnikId = uId.Value,
                Razlog = string.IsNullOrWhiteSpace(razlog) ? null : razlog.Trim(),
                DatumKreiranja = DateTime.UtcNow
            };

            await _forumRepository.AddKomentarPrijavaAsync(prijava);

            try
            {
                var prijavio = await _korisnikRepository.GetByIdAsync(uId.Value);
                var admins = (await _korisnikRepository.GetAllAsync())
                    .Where(k => string.Equals(k.Uloga?.Naziv, RoleNames.Administrator, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var detailsUrl = Url.Action(nameof(Details), "Forum", new { id = objavaId }, Request.Scheme) ?? string.Empty;

                var komentarTekst = WebUtility.HtmlEncode(komentar.Sadrzaj);
                var autorKomentara = WebUtility.HtmlEncode(komentar.Korisnik?.Ime + " " + komentar.Korisnik?.Prezime);
                var prijavioIme = WebUtility.HtmlEncode(prijavio?.Ime + " " + prijavio?.Prezime);
                var razlogTekst = WebUtility.HtmlEncode(prijava.Razlog ?? "(nije naveden)");

                var subject = "Prijava neadekvatnog komentara (forum)";
                var body = $@"
                    <h3>Prijavljen komentar na forumu</h3>
                    <p><strong>Autor komentara:</strong> {autorKomentara}</p>
                    <p><strong>Prijavio:</strong> {prijavioIme}</p>
                    <p><strong>Razlog:</strong> {razlogTekst}</p>
                    <p><strong>Sadržaj komentara:</strong></p>
                    <blockquote style=""border-left:4px solid #ddd;padding-left:12px;"">{komentarTekst}</blockquote>
                    <p><a href=""{detailsUrl}"">Otvori forum objavu</a></p>
                ";

                foreach (var admin in admins)
                {
                    if (!string.IsNullOrWhiteSpace(admin.Email))
                    {
                        await _emailService.SendEmailAsync(admin.Email, subject, body);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Neuspjelo slanje email obavijesti za prijavu komentara.");
            }

            TempData["SuccessMessage"] = "Komentar je prijavljen administratoru.";
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
            _cacheVersions.BumpForumVersion();

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
