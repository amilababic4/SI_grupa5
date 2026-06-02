using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
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
        private static readonly TimeSpan HomeRandomCacheTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan HomeRecoCacheTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan HomeNewsCacheTtl = TimeSpan.FromMinutes(3);
        private static readonly TimeSpan HomeEventsCacheTtl = TimeSpan.FromMinutes(2);

        public HomeController(
            IKnjigaRepository knjigaRepository,
            IBookRecommender bookRecommender,
            IDistributedCache cache,
            CacheVersionStore cacheVersions,
            IVijestRepository vijestRepository,
            IDogadjajRepository dogadjajRepository)
        {
            _knjigaRepository = knjigaRepository;
            _bookRecommender = bookRecommender;
            _cache = cache;
            _cacheVersions = cacheVersions;
            _vijestRepository = vijestRepository;
            _dogadjajRepository = dogadjajRepository;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new Models.HomeViewModel();

            // Dohvatamo nasumične 4 knjige za landing page
            var booksVersion = _cacheVersions.BooksVersion;
            var categoriesVersion = _cacheVersions.CategoriesVersion;
            var randomCacheKey = $"home_random_v1_{booksVersion}_{categoriesVersion}";
            var randomDtos = await _cache.GetOrCreateRecordAsync(randomCacheKey, HomeRandomCacheTtl, async () =>
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
            });
            viewModel.RandomKnjige = randomDtos;

            // Ako je korisnik prijavljen, dohvatamo preporuke
            if (User.Identity?.IsAuthenticated ?? false)
            {
                // Za sada koristimo dummy ID 1 ili dohvatamo pravi ID iz claimova ako postoji
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    var recoCacheKey = $"home_reco_{userId}_{booksVersion}_{categoriesVersion}";
                    var recoDtos = await _cache.GetOrCreateRecordAsync(recoCacheKey, HomeRecoCacheTtl, async () =>
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
                    });
                    viewModel.RecommendedKnjige = recoDtos;
                }
            }

            var newsCacheKey = $"home_news_v1_{_cacheVersions.NewsVersion}";
            var newsDtos = await _cache.GetOrCreateRecordAsync(newsCacheKey, HomeNewsCacheTtl, async () =>
            {
                var recent = await _vijestRepository.GetRecentAsync(4);
                return recent.Select(v => new Models.HomeVijestDto
                {
                    Id = v.Id,
                    Naslov = v.Naslov,
                    Sadrzaj = v.Sadrzaj,
                    Kategorija = v.Kategorija,
                    SlikaUrl = v.SlikaUrl,
                    DatumObjave = v.DatumObjave
                }).ToList();
            });
            viewModel.RecentVijesti = newsDtos;

            var todayKey = DateTime.Today.ToString("yyyyMMdd");
            var eventsCacheKey = $"home_events_v1_{_cacheVersions.EventsVersion}_{todayKey}";
            var eventDtos = await _cache.GetOrCreateRecordAsync(eventsCacheKey, HomeEventsCacheTtl, async () =>
            {
                var upcoming = await _dogadjajRepository.GetUpcomingAsync(DateTime.Today, 4);
                return upcoming.Select(d => new Models.HomeDogadjajDto
                {
                    Id = d.Id,
                    Naslov = d.Naslov,
                    Kategorija = d.Kategorija,
                    Datum = d.Datum,
                    Sat = d.Sat,
                    Lokacija = d.Lokacija
                }).ToList();
            });
            viewModel.UpcomingDogadjaji = eventDtos;

            return View(viewModel);
        }


        public IActionResult Error()
        {
            // TODO: Error handling view
            return View();
        }
    }
}

