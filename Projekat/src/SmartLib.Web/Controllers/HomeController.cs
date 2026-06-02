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

            var booksVersion = _cacheVersions.BooksVersion;
            Response.Headers.Append("X-Cache-Version", booksVersion.ToString());

            // Dohvatamo nasumične 4 knjige za landing page
            var randomCacheKey = CacheKeyBuilder.HomeRandomKey(booksVersion);
            var randomDtos = await _cache.GetOrCreateRecordAsync(
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
                }) ?? new List<KnjigaDto>();
            viewModel.RandomKnjige = randomDtos;

            // Ako je korisnik prijavljen, dohvatamo preporuke
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    var recoCacheKey = CacheKeyBuilder.HomeRecommendationsKey(booksVersion, userId);
                    var recoDtos = await _cache.GetOrCreateRecordAsync(
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
                        }) ?? new List<KnjigaDto>();
                    viewModel.RecommendedKnjige = recoDtos;
                }
            }

            var allVijesti = await _vijestRepository.GetAllAsync();
            viewModel.RecentVijesti = allVijesti.Take(4).ToList();

            var allDogadjaji = await _dogadjajRepository.GetAllAsync();
            viewModel.UpcomingDogadjaji = allDogadjaji
                .Where(d => d.Datum.Date >= DateTime.Today)
                .OrderBy(d => d.Datum)
                .Take(4)
                .ToList();

            return View(viewModel);
        }


        public IActionResult Error()
        {
            // TODO: Error handling view
            return View();
        }
    }
}

