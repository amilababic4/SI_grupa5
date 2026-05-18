using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Web.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace SmartLib.Web.Controllers
{
    [Authorize]
    public class KnjigaController : Controller
    {
        private readonly IKnjigaRepository _knjigaRepository;
        private readonly IPrimjerakRepository _primjerakRepository;
        private readonly IKategorijaRepository _kategorijaRepository;
        private readonly IZaduzenjeRepository _zaduzenjeRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KnjigaController> _logger;

        public KnjigaController(
            IKnjigaRepository knjigaRepository,
            IPrimjerakRepository primjerakRepository,
            IKategorijaRepository kategorijaRepository,
            IZaduzenjeRepository zaduzenjeRepository,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            IConfiguration configuration,
            ILogger<KnjigaController> logger)
        {
            _knjigaRepository = knjigaRepository;
            _primjerakRepository = primjerakRepository;
            _kategorijaRepository = kategorijaRepository;
            _zaduzenjeRepository = zaduzenjeRepository;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<IActionResult> Korice(string isbn, string size = "M")
        {
            if (string.IsNullOrEmpty(isbn)) return NotFound();

            var normalizedIsbn = NormalizeIsbn(isbn);
            var cacheKey = $"cover_{normalizedIsbn}_{size}";

            if (_cache.TryGetValue(cacheKey, out byte[]? cachedImage) && cachedImage != null)
            {
                return File(cachedImage, "image/jpeg");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var zoom = MapCoverZoom(size);
                var url = $"https://books.google.com/books/content?vid=ISBN:{normalizedIsbn}&printsec=frontcover&img=1&zoom={zoom}&source=gbs_api";
                
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    
                    // Cache for 24 hours in memory
                    _cache.Set(cacheKey, imageBytes, TimeSpan.FromHours(24));
                    
                    return File(imageBytes, "image/jpeg");
                }
            }
            catch
            {
                // Fallback or just let it fail
            }

            return NotFound();
        }

        public async Task<IActionResult> Index(string? naslov, string? autor, int page = 1)
        {
            const int pageSize = 16;
            if (page < 1) page = 1;

            var (knjige, ukupno) = await _knjigaRepository.GetPagedAsync(naslov, autor, page, pageSize);

            var dtos = knjige.Select(k => new KnjigaDto
            {
                Id = k.Id,
                Naslov = k.Naslov,
                Autor = k.Autor,
                Isbn = k.Isbn,
                Kategorija = k.Kategorija?.Naziv,
                Izdavac = k.Izdavac,
                GodinaIzdanja = k.GodinaIzdanja,
                BrojPrimjeraka = k.Primjerci.Count,
                BrojDostupnih = k.Primjerci.Count(p => p.Status == "dostupan")
            }).ToList();

            int ukupnoStrana = ukupno == 0 ? 1 : (int)Math.Ceiling((double)ukupno / pageSize);

            var model = new KatalogViewModel
            {
                Knjige = dtos,
                TrenutnaStrana = page,
                UkupnoStrana = ukupnoStrana,
                UkupnoStavki = ukupno,
                VelicinaStrane = pageSize,
                Naslov = naslov,
                Autor = autor
            };

            return View(model);
        }

        public async Task<IActionResult> Explore()
        {
            var userId = GetUserId();
            if (userId == null) return Forbid();

            var history = await _zaduzenjeRepository.GetHistoryByKorisnikAsync(userId.Value);
            var historyCategories = history
                .Select(z => z.Primjerak?.Knjiga?.Kategorija?.Naziv)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!.Trim())
                .ToList();

            var preferredCategories = historyCategories
                .GroupBy(n => n)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(3)
                .ToList();

            var allCategories = (await _kategorijaRepository.GetAllAsync())
                .Select(k => k.Naziv)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!preferredCategories.Any())
            {
                preferredCategories = allCategories
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(Math.Min(2, allCategories.Count))
                    .ToList();
            }

            var surpriseCategories = allCategories
                .Except(preferredCategories, StringComparer.OrdinalIgnoreCase)
                .OrderBy(_ => Guid.NewGuid())
                .Take(Math.Min(2, allCategories.Count))
                .ToList();

            const int totalTarget = 20;
            var preferredBudget = (int)Math.Ceiling(totalTarget * 0.8);
            var surpriseBudget = totalTarget - preferredBudget;

            var perPreferred = preferredCategories.Count == 0 ? preferredBudget : Math.Max(4, preferredBudget / preferredCategories.Count);
            var allRandom = await _knjigaRepository.GetRandomAsync(totalTarget * 2);
            var preferredBooks = allRandom.Where(b => b.Kategorija != null && preferredCategories.Contains(b.Kategorija.Naziv, StringComparer.OrdinalIgnoreCase)).Take(preferredBudget).ToList();
            var surpriseBooks = allRandom.Where(b => b.Kategorija != null && surpriseCategories.Contains(b.Kategorija.Naziv, StringComparer.OrdinalIgnoreCase)).Take(surpriseBudget).ToList();

            var selectedBooks = new List<Knjiga>();
            selectedBooks.AddRange(preferredBooks);
            selectedBooks.AddRange(surpriseBooks);

            var remaining = allRandom.Except(selectedBooks).Take(totalTarget - selectedBooks.Count).ToList();
            selectedBooks.AddRange(remaining);

            // Start fetching descriptions in parallel
            var descriptionTasks = selectedBooks.Select(book => FetchDescriptionFromGoogleBooksAsync(book.Isbn)).ToList();
            var descriptions = await Task.WhenAll(descriptionTasks);

            var rawCards = new List<ExploreCardViewModel>();
            for (int i = 0; i < selectedBooks.Count; i++)
            {
                var book = selectedBooks[i];
                var description = descriptions[i];

                var thumbnail = string.Empty;
                if (!string.IsNullOrWhiteSpace(book.Isbn))
                {
                    thumbnail = Url.Action("Korice", "Knjiga", new { isbn = book.Isbn, size = "L" }) ?? string.Empty;
                }
                bool isWildcard = book.Kategorija != null && surpriseCategories.Contains(book.Kategorija.Naziv, StringComparer.OrdinalIgnoreCase);

                rawCards.Add(new ExploreCardViewModel
                {
                    Title = string.IsNullOrWhiteSpace(book.Naslov) ? "Nepoznat naslov" : book.Naslov,
                    Authors = string.IsNullOrWhiteSpace(book.Autor) ? "Nepoznat autor" : book.Autor,
                    Category = book.Kategorija?.Naziv ?? "Ostalo",
                    Description = description,
                    ThumbnailUrl = thumbnail,
                    InfoLink = Url.Action("Details", "Knjiga", new { id = book.Id }) ?? string.Empty,
                    IsWildcard = isWildcard
                });
            }

            var uniqueCards = new List<ExploreCardViewModel>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var card in rawCards)
            {
                var key = $"{card.Title}|{card.Authors}";
                if (seen.Add(key)) uniqueCards.Add(card);
            }

            var finalCards = uniqueCards
                .OrderBy(_ => Random.Shared.Next())
                .Take(totalTarget)
                .ToList();

            var model = new ExploreViewModel
            {
                Cards = finalCards,
                PreferredCategories = preferredCategories,
                SurpriseCategories = surpriseCategories
            };

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCatalogRecommendation(string? category = null)
        {
            try
            {
                var books = await _knjigaRepository.GetRandomAsync(10);
                if (!books.Any()) return NotFound();

                Knjiga? book = null;
                if (!string.IsNullOrWhiteSpace(category))
                {
                    book = books.FirstOrDefault(b => 
                        b.Kategorija?.Naziv?.Equals(category.Trim(), StringComparison.OrdinalIgnoreCase) ?? false);
                }

                book ??= books.FirstOrDefault();

                if (book == null) return NotFound();

                return Json(new
                {
                    id = book.Id,
                    title = book.Naslov,
                    authors = book.Autor,
                    category = book.Kategorija?.Naziv ?? "Ostalo",
                    isbn = book.Isbn
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error fetching catalog recommendation: {Message}", ex.Message);
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var knjiga = await _knjigaRepository.GetByIdAsync(id);
            if (knjiga == null) return NotFound();

            var dto = new KnjigaDto
            {
                Id = knjiga.Id,
                Naslov = knjiga.Naslov,
                Autor = knjiga.Autor,
                Isbn = knjiga.Isbn,
                Kategorija = knjiga.Kategorija?.Naziv,
                Izdavac = knjiga.Izdavac,
                GodinaIzdanja = knjiga.GodinaIzdanja,
                BrojPrimjeraka = knjiga.Primjerci.Count,
                BrojDostupnih = knjiga.Primjerci.Count(p => p.Status == "dostupan")
            };

            ViewBag.Primjerci = knjiga.Primjerci.OrderBy(p => p.InventarniBroj).ToList();

            return View(dto);
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateKategorije();
            return View(new KnjigaCreateDto());
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        public async Task<IActionResult> Create(KnjigaCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateKategorije(model.KategorijaId);
                return View(model);
            }

            var normalizedIsbn = NormalizeIsbn(model.Isbn);
            if (!IsValidIsbn(normalizedIsbn))
            {
                ModelState.AddModelError(nameof(model.Isbn), "ISBN nije u ispravnom formatu. Unesite 10 ili 13 cifara (sa crticama ili bez).");
                await PopulateKategorije(model.KategorijaId);
                return View(model);
            }

            var existing = await _knjigaRepository.GetByIsbnAsync(normalizedIsbn);
            if (existing != null)
            {
                ModelState.AddModelError(nameof(model.Isbn), "Knjiga sa ovim ISBN-om već postoji u katalogu.");
                await PopulateKategorije(model.KategorijaId);
                return View(model);
            }

            var kategorija = await _kategorijaRepository.GetByIdAsync(model.KategorijaId);
            if (kategorija == null)
            {
                ModelState.AddModelError(nameof(model.KategorijaId), "Odabrana kategorija nije validna.");
                await PopulateKategorije(model.KategorijaId);
                return View(model);
            }

            var knjiga = new Knjiga
            {
                Naslov = model.Naslov.Trim(),
                Autor = model.Autor.Trim(),
                Isbn = normalizedIsbn,
                KategorijaId = model.KategorijaId,
                Izdavac = model.Izdavac?.Trim(),
                GodinaIzdanja = model.GodinaIzdanja
            };

            var savedKnjiga = await _knjigaRepository.CreateAsync(knjiga);

            for (int i = 0; i < model.BrojPrimjeraka; i++)
            {
                await _primjerakRepository.CreateAsync(new Primjerak
                {
                    KnjigaId = savedKnjiga.Id,
                    InventarniBroj = $"INV-{savedKnjiga.Id}-{i + 1:D3}",
                    Status = "dostupan",
                    DatumNabave = DateTime.UtcNow
                });
            }

            TempData["SuccessMessage"] = $"Knjiga \"{savedKnjiga.Naslov}\" je uspješno dodana u katalog.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var knjiga = await _knjigaRepository.GetByIdAsync(id);
            if (knjiga == null) return NotFound();

            var model = new KnjigaEditDto
            {
                Id = knjiga.Id,
                Naslov = knjiga.Naslov,
                Autor = knjiga.Autor,
                KategorijaId = knjiga.KategorijaId,
                Izdavac = knjiga.Izdavac,
                GodinaIzdanja = knjiga.GodinaIzdanja
            };

            ViewBag.Isbn = knjiga.Isbn;
            await PopulateKategorije(knjiga.KategorijaId);
            return View(model);
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        public async Task<IActionResult> Edit(KnjigaEditDto model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateKategorije(model.KategorijaId);
                return View(model);
            }

            var knjiga = await _knjigaRepository.GetByIdAsync(model.Id);
            if (knjiga == null) return NotFound();

            var kategorija = await _kategorijaRepository.GetByIdAsync(model.KategorijaId);
            if (kategorija == null)
            {
                ModelState.AddModelError(nameof(model.KategorijaId), "Odabrana kategorija nije validna.");
                await PopulateKategorije(model.KategorijaId);
                return View(model);
            }

            knjiga.Naslov = model.Naslov.Trim();
            knjiga.Autor = model.Autor.Trim();
            knjiga.KategorijaId = model.KategorijaId;
            knjiga.Izdavac = model.Izdavac?.Trim();
            knjiga.GodinaIzdanja = model.GodinaIzdanja;

            await _knjigaRepository.UpdateAsync(knjiga);

            TempData["SuccessMessage"] = $"Podaci knjige \"{knjiga.Naslov}\" su uspješno ažurirani.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Bibliotekar,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken] // Sigurnosna zaštita koju već imaš u View-u
        public async Task<IActionResult> Delete(int id)
        {
            // 1. Provjera aktivnih zaduženja (US-28)
            if (await _knjigaRepository.HasActiveLoansAsync(id))
            {
                TempData["ErrorMessage"] = "Knjiga ima aktivna zaduženja i ne može biti obrisana.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var success = await _knjigaRepository.DeleteAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "Knjiga je uspješno obrisana iz kataloga."; // US-25
                }
                else
                {
                    TempData["ErrorMessage"] = "Došlo je do greške prilikom brisanja knjige.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Sistemska greška: " + ex.Message;
            }

            return RedirectToAction(nameof(Index)); // US-29 (Osvježavanje liste)
        }

        private async Task PopulateKategorije(int? selectedId = null)
        {
            var kategorije = await _kategorijaRepository.GetAllAsync();
            ViewBag.Kategorije = new SelectList(kategorije, "Id", "Naziv", selectedId);
        }

        private int? GetUserId()
        {
            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idValue, out var id)) return id;
            return null;
        }







        private static int MapCoverZoom(string size) => size switch
        {
            "S" => 1,
            "L" => 3,
            _ => 2
        };

        private async Task<string> FetchDescriptionFromGoogleBooksAsync(string? isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn)) return "Opis nije dostupan.";
            var normalizedIsbn = NormalizeIsbn(isbn);
            
            var apiKey = _configuration["GOOGLE_BOOKS_API_KEY"] ?? _configuration["GoogleBooks:ApiKey"];
            var url = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{normalizedIsbn}";
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                url += $"&key={Uri.EscapeDataString(apiKey)}";
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("items", out var items) && items.GetArrayLength() > 0)
                    {
                        var firstItem = items[0];
                        if (firstItem.TryGetProperty("volumeInfo", out var volumeInfo))
                        {
                            var description = GetJsonString(volumeInfo, "description");
                            return TrimText(description, 260);
                        }
                    }
                }
            }
            catch
            {
                // Ignoriši greške
            }

            return "Opis nije dostupan.";
        }

        private static string? GetJsonString(JsonElement element, string propertyName)
            => element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String
                ? prop.GetString()
                : null;

        private static string TrimText(string? text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text)) return "Opis nije dostupan.";
            var trimmed = text.Trim();
            return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength].TrimEnd() + "...";
        }

        private static string NormalizeIsbn(string isbn)
            => isbn.Replace("-", "").Replace(" ", "").Trim();

        private static bool IsValidIsbn(string isbn)
        {
            if (string.IsNullOrEmpty(isbn)) return false;
            if (isbn.Length == 13) return isbn.All(char.IsDigit);
            if (isbn.Length == 10) return isbn[..9].All(char.IsDigit) && (char.IsDigit(isbn[9]) || isbn[9] == 'X');
            return false;
        }
    }
}
