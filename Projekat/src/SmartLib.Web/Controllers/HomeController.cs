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

            // Dohvatamo nasumične 4 knjige za landing page
            var booksVersion = _cacheVersions.BooksVersion;
            var categoriesVersion = _cacheVersions.CategoriesVersion;
            var randomCacheKey = $"home_random_v1_{booksVersion}_{categoriesVersion}";
            var randomDtos = await _cache.GetRecordAsync<List<KnjigaDto>>(randomCacheKey);
            if (randomDtos == null)
            {
                var randomKnjige = await _knjigaRepository.GetRandomAsync(4);
                randomDtos = randomKnjige.Select(k => new KnjigaDto
                {
                    Id = k.Id,
                    Naslov = k.Naslov,
                    Autor = k.Autor,
                    Isbn = k.Isbn,
                    Kategorija = k.Kategorija?.Naziv,
                    GodinaIzdanja = k.GodinaIzdanja
                }).ToList();

                await _cache.SetRecordAsync(randomCacheKey, randomDtos, TimeSpan.FromMinutes(5));
            }
            viewModel.RandomKnjige = randomDtos;

            // Ako je korisnik prijavljen, dohvatamo preporuke
            if (User.Identity?.IsAuthenticated ?? false)
            {
                // Za sada koristimo dummy ID 1 ili dohvatamo pravi ID iz claimova ako postoji
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    var recoCacheKey = $"home_reco_{userId}_{booksVersion}_{categoriesVersion}";
                    var recoDtos = await _cache.GetRecordAsync<List<KnjigaDto>>(recoCacheKey);
                    if (recoDtos == null)
                    {
                        var recommended = await _bookRecommender.GetRecommendationsForUserAsync(userId);
                        recoDtos = recommended.Select(k => new KnjigaDto
                        {
                            Id = k.Id,
                            Naslov = k.Naslov,
                            Autor = k.Autor,
                            Isbn = k.Isbn,
                            Kategorija = k.Kategorija?.Naziv,
                            GodinaIzdanja = k.GodinaIzdanja
                        }).ToList();

                        await _cache.SetRecordAsync(recoCacheKey, recoDtos, TimeSpan.FromMinutes(5));
                    }
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

