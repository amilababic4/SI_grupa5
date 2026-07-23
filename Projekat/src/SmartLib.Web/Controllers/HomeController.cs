using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using SmartLib.Core.Interfaces;
using SmartLib.Core.DTOs;
using SmartLib.Infrastructure.Services;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Početna stranica i opće rute
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IKnjigaRepository _knjigaRepository;
        private readonly IBookRecommender _bookRecommender;
        private readonly IDistributedCache _cache;
        private readonly CacheVersionStore _cacheVersions;
        private readonly IVijestRepository _vijestRepository;
        private readonly IDogadjajRepository _dogadjajRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly SingleFlightCache _singleFlight;
        private static readonly TimeSpan BrowserCacheTtl = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan BrowserCacheMaxStale = TimeSpan.FromMinutes(1);

        public HomeController(
            IKnjigaRepository knjigaRepository,
            IBookRecommender bookRecommender,
            IDistributedCache cache,
            CacheVersionStore cacheVersions,
            IVijestRepository vijestRepository,
            IDogadjajRepository dogadjajRepository,
            IMemoryCache? memoryCache,
            SingleFlightCache singleFlight)
        {
            _knjigaRepository = knjigaRepository;
            _bookRecommender = bookRecommender;
            _cache = cache;
            _cacheVersions = cacheVersions;
            _vijestRepository = vijestRepository;
            _dogadjajRepository = dogadjajRepository;
            _memoryCache = memoryCache ?? new MemoryCache(new MemoryCacheOptions());
            _singleFlight = singleFlight;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new Models.HomeViewModel();

            var booksVersion = _cacheVersions.BooksVersion;
            Response.Headers.Append("X-Cache-Version", booksVersion.ToString());

            // Dohvatamo nasumične 4 knjige za landing page (single-flight: samo 1 request kreira cache)
            var randomCacheKey = CacheKeyBuilder.HomeRandomKey(booksVersion);
            var randomDtos = await _singleFlight.GetOrCreateAsync(
                randomCacheKey,
                TimeSpan.FromMinutes(3),
                async () =>
                {
                    var randomKnjige = await _knjigaRepository.GetRandomAsync(4);
                    return randomKnjige.Select(k => new KnjigaDto
                    {
                        Id = k.Id,
                        Naslov = k.Naslov,
                        Autor = k.Autor,
                        Isbn = k.Isbn,
                        Kategorija = k.Kategorija?.Naziv,
                        GodinaIzdanja = k.GodinaIzdanja
                    }).ToList();
                },
                _cache) ?? new List<KnjigaDto>();
            viewModel.RandomKnjige = randomDtos;

            // Ako je korisnik prijavljen, dohvatamo preporuke
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    var recoCacheKey = CacheKeyBuilder.HomeRecommendationsKey(booksVersion, userId);
                    var recoDtos = await _singleFlight.GetOrCreateAsync(
                        recoCacheKey,
                        TimeSpan.FromMinutes(2),
                        async () =>
                        {
                            var recommended = await _bookRecommender.GetRecommendationsForUserAsync(userId);
                            return recommended.Select(k => new KnjigaDto
                            {
                                Id = k.Id,
                                Naslov = k.Naslov,
                                Autor = k.Autor,
                                Isbn = k.Isbn,
                                Kategorija = k.Kategorija?.Naziv,
                                GodinaIzdanja = k.GodinaIzdanja
                            }).ToList();
                        },
                        _cache) ?? new List<KnjigaDto>();
                    viewModel.RecommendedKnjige = recoDtos;
                }
            }

            var allVijesti = await _cache.GetOrCreateRecordAsync(
                _memoryCache,
                CacheKeyBuilder.HomeVijestiKey(booksVersion),
                TimeSpan.FromMinutes(5),
                TimeSpan.FromMinutes(2),
                async () => (await _vijestRepository.GetAllAsync()).ToList()) ?? new List<Core.Models.Vijest>();
            viewModel.RecentVijesti = allVijesti.Take(4).ToList();

            var allDogadjaji = await _cache.GetOrCreateRecordAsync(
                _memoryCache,
                CacheKeyBuilder.HomeDogadjajiKey(booksVersion),
                TimeSpan.FromMinutes(5),
                TimeSpan.FromMinutes(2),
                async () => (await _dogadjajRepository.GetAllAsync()).ToList()) ?? new List<Core.Models.Dogadjaj>();
            viewModel.UpcomingDogadjaji = allDogadjaji
                .Where(d => d.Datum.Date >= DateTime.Today)
                .OrderBy(d => d.Datum)
                .Take(4)
                .ToList();

            ApplyBrowserCacheHeaders();

            return View(viewModel);
        }

        private void ApplyBrowserCacheHeaders()
        {
            Response.Headers[HeaderNames.CacheControl] = $"private, max-age={(int)BrowserCacheTtl.TotalSeconds}, stale-while-revalidate={(int)BrowserCacheMaxStale.TotalSeconds}";
            Response.Headers[HeaderNames.Vary] = HeaderNames.Cookie;
        }


        public IActionResult Error()
        {
            // TODO: Error handling view
            return View();
        }
    }
}

