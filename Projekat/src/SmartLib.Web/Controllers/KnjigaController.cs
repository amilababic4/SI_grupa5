using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Web.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using SmartLib.Infrastructure.Services;

namespace SmartLib.Web.Controllers
{
    [Authorize]
    public class KnjigaController : Controller
    {
        private readonly IKnjigaRepository _knjigaRepository;
        private readonly IPrimjerakRepository _primjerakRepository;
        private readonly IKategorijaRepository _kategorijaRepository;
        private readonly IZaduzenjeRepository _zaduzenjeRepository;
        private readonly IRezervacijaRepository _rezervacijaRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KnjigaController> _logger;
        private readonly CacheVersionStore _cacheVersions;
        private static readonly TimeSpan ExploreCacheTtl = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan CatalogRecommendationCacheTtl = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan BookIndexCacheTtl = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan BookDetailsCacheTtl = TimeSpan.FromMinutes(10);

        public KnjigaController(
            IKnjigaRepository knjigaRepository,
            IPrimjerakRepository primjerakRepository,
            IKategorijaRepository kategorijaRepository,
            IZaduzenjeRepository zaduzenjeRepository,
            IRezervacijaRepository rezervacijaRepository,
            IHttpClientFactory httpClientFactory,
            IDistributedCache cache,
            IConfiguration configuration,
            ILogger<KnjigaController> logger,
            CacheVersionStore cacheVersions)
        {
            _knjigaRepository = knjigaRepository;
            _primjerakRepository = primjerakRepository;
            _kategorijaRepository = kategorijaRepository;
            _zaduzenjeRepository = zaduzenjeRepository;
            _rezervacijaRepository = rezervacijaRepository;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
            _cacheVersions = cacheVersions;
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<IActionResult> Korice(string isbn, string size = "M")
        {
            if (string.IsNullOrEmpty(isbn)) return NotFound();

            var normalizedIsbn = NormalizeIsbn(isbn);
            var cacheKey = $"cover_{normalizedIsbn}_{size}";

            var cachedImage = await _cache.GetBytesAsync(cacheKey);
            if (cachedImage != null && cachedImage.Length > 0)
                return File(cachedImage, "image/jpeg");

            try
            {
                var upperSize = string.IsNullOrEmpty(size) ? "M" : size.ToUpper();
                if (upperSize != "S" && upperSize != "M" && upperSize != "L")
                {
                    upperSize = "M";
                }

                var client = _httpClientFactory.CreateClient();
                
                // Pokušaj 1: Open Library
                var openLibraryUrl = $"https://covers.openlibrary.org/b/isbn/{normalizedIsbn}-{upperSize}.jpg?default=false";
                var olResponse = await client.GetAsync(openLibraryUrl);

                if (olResponse.IsSuccessStatusCode)
                {
                    var imageBytes = await olResponse.Content.ReadAsByteArrayAsync();
                    // OpenLibrary često vraća mali placeholder sliku sa tekstom "Image Not Available" 
                    // ili prazan piksel. Te slike su obično manje od 3KB.
                    if (imageBytes != null && imageBytes.Length > 3000)
                    {
                        await _cache.SetBytesAsync(cacheKey, imageBytes, TimeSpan.FromHours(24));
                        return File(imageBytes, "image/jpeg");
                    }
                }

                // Pokušaj 2: Google Books API
                var apiKey = _configuration["GOOGLE_BOOKS_API_KEY"] ?? _configuration["GoogleBooks:ApiKey"];
                var googleUrl = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{normalizedIsbn}";
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    googleUrl += $"&key={Uri.EscapeDataString(apiKey)}";
                }

                var googleResponse = await client.GetAsync(googleUrl);
                if (googleResponse.IsSuccessStatusCode)
                {
                    var json = await googleResponse.Content.ReadAsStringAsync();
                    using var doc = System.Text.Json.JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("items", out var items) && items.GetArrayLength() > 0)
                    {
                        var volumeInfo = items[0].GetProperty("volumeInfo");
                        if (volumeInfo.TryGetProperty("imageLinks", out var imageLinks))
                        {
                            if (imageLinks.TryGetProperty("thumbnail", out var thumbUrl))
                            {
                                var url = thumbUrl.GetString()?.Replace("http://", "https://");
                                if (!string.IsNullOrEmpty(url))
                                {
                                    var gbResponse = await client.GetAsync(url);
                                    if (gbResponse.IsSuccessStatusCode)
                                    {
                                        var gbBytes = await gbResponse.Content.ReadAsByteArrayAsync();
                                        // Google Books vraća onaj sivi placeholder sa linijama koji je obično oko 2-3 KB.
                                        // Odbacujemo sve ispod 4500 bajtova kako bismo izbjegli placeholder.
                                        if (gbBytes != null && gbBytes.Length > 4500)
                                        {
                                            await _cache.SetBytesAsync(cacheKey, gbBytes, TimeSpan.FromHours(24));
                                            return File(gbBytes, "image/jpeg");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Greška prilikom dohvatanja korice za ISBN {Isbn}: {Message}", normalizedIsbn, ex.Message);
            }

            // FALLBACK SVG (ako API nema sliku ili dođe do greške)
            var fallbackSvg = $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""300"" height=""450"" viewBox=""0 0 300 450"">
                <rect width=""300"" height=""450"" fill=""#1e293b""/>
                <rect x=""15"" y=""15"" width=""270"" height=""420"" fill=""none"" stroke=""#334155"" stroke-width=""2""/>
                <text x=""150"" y=""200"" font-family=""sans-serif"" font-size=""24"" fill=""#64748b"" text-anchor=""middle"">Slika nije</text>
                <text x=""150"" y=""240"" font-family=""sans-serif"" font-size=""24"" fill=""#64748b"" text-anchor=""middle"">dostupna</text>
            </svg>";
            var svgBytes = System.Text.Encoding.UTF8.GetBytes(fallbackSvg);
            return File(svgBytes, "image/svg+xml");
        }

        public async Task<IActionResult> Index(string? naslov, string? autor, int page = 1, int pageSize = 16)
        {
            if (page < 1) page = 1;

            if (pageSize <= 0) pageSize = 16;

            var booksVersion = _cacheVersions.BooksVersion;
            var categoriesVersion = _cacheVersions.CategoriesVersion;
            var titleKey = NormalizeCachePart(naslov);
            var authorKey = NormalizeCachePart(autor);
            var cacheKey = $"books_index_v1_{booksVersion}_{categoriesVersion}_{titleKey}_{authorKey}_{page}_{pageSize}";

            var cachedModel = await _cache.GetRecordAsync<KatalogViewModel>(cacheKey);
            if (cachedModel != null)
                return View(cachedModel);

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
                Opis = k.Opis,
                BrojPrimjeraka = k.Primjerci.Count,
                BrojDostupnih = k.Primjerci.Count(p => p.Status == "dostupan"),
                ProsjecnaOcjena = k.Recenzije != null && k.Recenzije.Any() ? k.Recenzije.Average(r => r.Ocjena) : 0,
                BrojRecenzija = k.Recenzije?.Count ?? 0
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

            await _cache.SetRecordAsync(cacheKey, model, BookIndexCacheTtl);

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

            const int totalTarget = 10;
            var preferredBudget = (int)Math.Ceiling(totalTarget * 0.8);
            var surpriseBudget = totalTarget - preferredBudget;

            // Dohvatamo sve knjige i shufflujemo ih svježe svaki put
            var allRandom = await _knjigaRepository.GetRandomAsync(50);
            
            // Osiguravamo unikatnost knjiga po Naslovu i Autoru
            var uniquePool = new List<Knjiga>();
            var seenPool = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var b in allRandom)
            {
                if (string.IsNullOrWhiteSpace(b.Naslov) || string.IsNullOrWhiteSpace(b.Autor)) continue;
                var key = $"{b.Naslov.Trim()}|{b.Autor.Trim()}";
                if (seenPool.Add(key))
                {
                    uniquePool.Add(b);
                }
            }

            // Pravi shuffle (Fisher-Yates) za potpunu randomizaciju svakog zahtjeva
            for (int i = uniquePool.Count - 1; i > 0; i--)
            {
                int j = Random.Shared.Next(i + 1);
                (uniquePool[i], uniquePool[j]) = (uniquePool[j], uniquePool[i]);
            }

            var preferredBooks = uniquePool
                .Where(b => b.Kategorija != null && preferredCategories.Contains(b.Kategorija.Naziv, StringComparer.OrdinalIgnoreCase))
                .Take(preferredBudget)
                .ToList();

            var surpriseBooks = uniquePool
                .Except(preferredBooks)
                .Where(b => b.Kategorija != null && surpriseCategories.Contains(b.Kategorija.Naziv, StringComparer.OrdinalIgnoreCase))
                .Take(surpriseBudget)
                .ToList();

            var selectedBooks = new List<Knjiga>();
            selectedBooks.AddRange(preferredBooks);
            selectedBooks.AddRange(surpriseBooks);

            var remaining = uniquePool
                .Except(selectedBooks)
                .Take(totalTarget - selectedBooks.Count)
                .ToList();
            selectedBooks.AddRange(remaining);

            var descriptions = new List<string>();
            foreach (var book in selectedBooks)
            {
                var opis = book.Opis;
                if (!string.IsNullOrWhiteSpace(opis))
                {
                    var transCacheKey = $"desc_trans_{book.Id}";
                    var cachedTrans = await _cache.GetRecordAsync<string>(transCacheKey);
                    if (!string.IsNullOrWhiteSpace(cachedTrans))
                    {
                        descriptions.Add(cachedTrans);
                    }
                    else
                    {
                        var translated = await TranslateToBosnianAsync(opis);
                        var finalText = string.IsNullOrWhiteSpace(translated) ? opis : translated;
                        await _cache.SetRecordAsync(transCacheKey, finalText, TimeSpan.FromHours(24));
                        descriptions.Add(finalText);
                    }
                }
                else
                {
                    var desc = await FetchDescriptionFromGoogleBooksAsync(book.Isbn);
                    descriptions.Add(desc);
                    await Task.Delay(50); // Kratka pauza da ne bismo preplavili Google API
                }
            }

            var rawCards = new List<ExploreCardViewModel>();
            for (int i = 0; i < selectedBooks.Count; i++)
            {
                var book = selectedBooks[i];
                var description = descriptions[i];

                // Prefer serving covers from our internal Korice endpoint (consistent caching/fallbacks).
                var thumbnail = string.Empty;
                if (!string.IsNullOrWhiteSpace(book.Isbn))
                {
                    thumbnail = Url.Action("Korice", "Knjiga", new { isbn = book.Isbn, size = "L" }) ?? string.Empty;
                }
                else if (!string.IsNullOrWhiteSpace(book.SlikaUrl))
                {
                    // Fallback to stored external image URL if ISBN is not available
                    thumbnail = book.SlikaUrl;
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
                var normalizedCategory = string.IsNullOrWhiteSpace(category)
                    ? "all"
                    : category.Trim().ToLowerInvariant();
                var cacheKey = $"catalog_reco_{normalizedCategory}_{_cacheVersions.BooksVersion}_{_cacheVersions.CategoriesVersion}";

                var cached = await _cache.GetRecordAsync<CatalogRecommendationResponse>(cacheKey);
                if (cached != null)
                    return Json(cached);

                var books = await _knjigaRepository.GetRandomAsync(50);
                if (!books.Any()) return NotFound();

                // Avoid always recommending the demo book when possible.
                var nonDemo = books
                    .Where(b => !string.Equals(b.Naslov, "Demo knjiga", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                if (nonDemo.Any())
                {
                    books = nonDemo;
                }

                Knjiga? book = null;
                if (!string.IsNullOrWhiteSpace(category))
                {
                    book = books.FirstOrDefault(b => 
                        b.Kategorija?.Naziv?.Equals(category.Trim(), StringComparison.OrdinalIgnoreCase) ?? false);
                }

                book ??= books.OrderBy(_ => Random.Shared.Next()).FirstOrDefault();

                if (book == null) return NotFound();

                var response = new CatalogRecommendationResponse
                {
                    Id = book.Id,
                    Title = book.Naslov,
                    Authors = book.Autor,
                    Category = book.Kategorija?.Naziv ?? "Ostalo",
                    Isbn = book.Isbn
                };

                await _cache.SetRecordAsync(cacheKey, response, CatalogRecommendationCacheTtl);
                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error fetching catalog recommendation: {Message}", ex.Message);
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> Details(int id, [FromServices] IRecenzijaRepository recenzijaRepository)
        {
            var cacheKey = $"book_details_v1_{_cacheVersions.BooksVersion}_{_cacheVersions.CategoriesVersion}_{id}";
            var cachedEntry = await _cache.GetRecordAsync<KnjigaDetailsCacheEntry>(cacheKey);
            if (cachedEntry != null)
            {
                if (string.IsNullOrWhiteSpace(cachedEntry.Dto.Opis))
                {
                    var normalizedIsbn = NormalizeIsbn(cachedEntry.Dto.Isbn);
                    var descCacheKey = $"desc_{normalizedIsbn}";
                    var cachedDesc = await _cache.GetRecordAsync<string>(descCacheKey);
                    if (!string.IsNullOrWhiteSpace(cachedDesc))
                        cachedEntry.Dto.Opis = cachedDesc;
                }

                if (!string.IsNullOrWhiteSpace(cachedEntry.Dto.Opis))
                {
                    var transCacheKey = $"desc_trans_{cachedEntry.Dto.Id}";
                    var translatedDesc = await _cache.GetRecordAsync<string>(transCacheKey);
                    if (!string.IsNullOrWhiteSpace(translatedDesc))
                    {
                        cachedEntry.Dto.Opis = translatedDesc;
                    }
                    else
                    {
                        var translated = await TranslateToBosnianAsync(cachedEntry.Dto.Opis);
                        if (!string.IsNullOrWhiteSpace(translated))
                        {
                            cachedEntry.Dto.Opis = translated;
                            await _cache.SetRecordAsync(transCacheKey, translated, TimeSpan.FromHours(24));
                        }
                    }
                }
                
                ViewBag.Primjerci = cachedEntry.Primjerci;
                await SetRezervacijaViewBag(id);
                ViewBag.Recenzije = await recenzijaRepository.GetByKnjigaIdAsync(id);
                return View(cachedEntry.Dto);
            }

            var knjiga = await _knjigaRepository.GetByIdAsync(id);
            if (knjiga == null) return NotFound();

            var opis = knjiga.Opis;
            if (string.IsNullOrWhiteSpace(opis))
            {
                var normalizedIsbn = NormalizeIsbn(knjiga.Isbn);
                var descCacheKey = $"desc_{normalizedIsbn}";
                var cachedDesc = await _cache.GetRecordAsync<string>(descCacheKey);
                if (!string.IsNullOrWhiteSpace(cachedDesc))
                    opis = cachedDesc;
            }

            if (!string.IsNullOrWhiteSpace(opis))
            {
                var transCacheKey = $"desc_trans_{knjiga.Id}";
                var translatedDesc = await _cache.GetRecordAsync<string>(transCacheKey);
                if (!string.IsNullOrWhiteSpace(translatedDesc))
                {
                    opis = translatedDesc;
                }
                else
                {
                    var translated = await TranslateToBosnianAsync(opis);
                    if (!string.IsNullOrWhiteSpace(translated))
                    {
                        opis = translated;
                        await _cache.SetRecordAsync(transCacheKey, translated, TimeSpan.FromHours(24));
                    }
                }
            }

            var dto = new KnjigaDto
            {
                Id = knjiga.Id,
                Naslov = knjiga.Naslov,
                Autor = knjiga.Autor,
                Isbn = knjiga.Isbn,
                Kategorija = knjiga.Kategorija?.Naziv,
                Izdavac = knjiga.Izdavac,
                GodinaIzdanja = knjiga.GodinaIzdanja,
                Opis = opis,
                BrojPrimjeraka = knjiga.Primjerci.Count,
                BrojDostupnih = knjiga.Primjerci.Count(p => p.Status == "dostupan")
            };

            var primjerci = knjiga.Primjerci.OrderBy(p => p.InventarniBroj).ToList();
            ViewBag.Primjerci = primjerci;
            ViewBag.Recenzije = await recenzijaRepository.GetByKnjigaIdAsync(id);

            await _cache.SetRecordAsync(cacheKey, new KnjigaDetailsCacheEntry
            {
                Dto = dto,
                Primjerci = primjerci
            }, BookDetailsCacheTtl);

            await SetRezervacijaViewBag(id);
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
            _cacheVersions.BumpBooksVersion();
            _cacheVersions.BumpCategoriesVersion();
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
            _cacheVersions.BumpBooksVersion();
            _cacheVersions.BumpCategoriesVersion();
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

            var changed = false;
            try
            {
                var success = await _knjigaRepository.DeleteAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "Knjiga je uspješno obrisana iz kataloga."; // US-25
                    changed = true;
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

            if (changed)
            {
                _cacheVersions.BumpBooksVersion();
                _cacheVersions.BumpCategoriesVersion();
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
            
            var cacheKey = $"desc_{normalizedIsbn}";
            var cachedDesc = await _cache.GetRecordAsync<string>(cacheKey);
            if (!string.IsNullOrEmpty(cachedDesc))
                return cachedDesc;
            
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
                            var trimmed = TrimText(description, 260);
                            
                            if (trimmed != "Opis nije dostupan.")
                            {
                                var translated = await TranslateToBosnianAsync(trimmed);
                                var finalText = string.IsNullOrWhiteSpace(translated) ? trimmed : translated;

                                await _cache.SetRecordAsync(cacheKey, finalText, TimeSpan.FromHours(24));
                                try
                                {
                                    await _knjigaRepository.TryUpdateOpisByIsbnAsync(normalizedIsbn, finalText);
                                    // Ne bumpamo BooksVersion jer bi to stalno invalidiralo keš za sve korisnike 
                                    // dok se u pozadini prevode opisi knjiga.
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning("Greška pri snimanju opisa u bazu za ISBN {Isbn}: {Message}", normalizedIsbn, ex.Message);
                                }

                                return finalText;
                            }
                            return trimmed;
                        }
                    }
                }
            }
            catch
            {
                // Ignoriši greške
            }

            await _cache.SetRecordAsync(cacheKey, "Opis nije dostupan.", TimeSpan.FromMinutes(10));
            return "Opis nije dostupan.";
        }

        private async Task<string> TranslateToBosnianAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text == "Opis nije dostupan.") return text;

            try
            {
                var client = _httpClientFactory.CreateClient();
                
                // Postavljamo User-Agent jer Google Translate blokira zahtjeve bez njega (403 Forbidden)
                client.DefaultRequestHeaders.UserAgent.Clear();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=bs&dt=t&q={Uri.EscapeDataString(text)}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                    {
                        var firstArray = root[0];
                        if (firstArray.ValueKind == JsonValueKind.Array)
                        {
                            var translatedParts = new List<string>();
                            foreach (var element in firstArray.EnumerateArray())
                            {
                                if (element.ValueKind == JsonValueKind.Array && element.GetArrayLength() > 0)
                                {
                                    var part = element[0].GetString();
                                    if (!string.IsNullOrEmpty(part))
                                    {
                                        translatedParts.Add(part);
                                    }
                                }
                            }
                            if (translatedParts.Any())
                            {
                                return string.Join(" ", translatedParts).Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Greška pri prevođenju teksta na bosanski: {Message}", ex.Message);
            }

            return text; // Vrati originalni tekst ako prevođenje ne uspije
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

        private static string NormalizeCachePart(string? value)
            => string.IsNullOrWhiteSpace(value) ? "all" : value.Trim().ToLowerInvariant();

        private sealed class CatalogRecommendationResponse
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Authors { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Isbn { get; set; } = string.Empty;
        }

        private sealed class KnjigaDetailsCacheEntry
        {
            public KnjigaDto Dto { get; set; } = new();
            public List<Primjerak> Primjerci { get; set; } = new();
        }

        private static bool IsValidIsbn(string isbn)
        {
            if (string.IsNullOrEmpty(isbn)) return false;
            if (isbn.Length == 13) return isbn.All(char.IsDigit);
            if (isbn.Length == 10) return isbn[..9].All(char.IsDigit) && (char.IsDigit(isbn[9]) || isbn[9] == 'X');
            return false;
        }

        private async Task SetRezervacijaViewBag(int knjigaId)
        {
            if (!User.IsInRole(RoleNames.Clan)) return;
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var uid)) return;
            ViewBag.ImaAktivnuRezervaciju = await _rezervacijaRepository.HasActiveAsync(uid, knjigaId);
        }
    }
}
