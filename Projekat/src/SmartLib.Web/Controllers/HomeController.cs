using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;
        private readonly CacheVersionStore _cacheVersions;

        public HomeController(
            IKnjigaRepository knjigaRepository,
            IBookRecommender bookRecommender,
            IMemoryCache cache,
            CacheVersionStore cacheVersions)
        {
            _knjigaRepository = knjigaRepository;
            _bookRecommender = bookRecommender;
            _cache = cache;
            _cacheVersions = cacheVersions;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new Models.HomeViewModel();

            // Dohvatamo nasumične 4 knjige za landing page
            var booksVersion = _cacheVersions.BooksVersion;
            var categoriesVersion = _cacheVersions.CategoriesVersion;
            var randomCacheKey = $"home_random_v1_{booksVersion}_{categoriesVersion}";
            if (!_cache.TryGetValue(randomCacheKey, out List<KnjigaDto>? randomDtos))
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

                _cache.Set(randomCacheKey, randomDtos, TimeSpan.FromMinutes(5));
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
                    if (!_cache.TryGetValue(recoCacheKey, out List<KnjigaDto>? recoDtos))
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

                        _cache.Set(recoCacheKey, recoDtos, TimeSpan.FromMinutes(5));
                    }
                    viewModel.RecommendedKnjige = recoDtos;
                }
            }

            return View(viewModel);
        }


        public IActionResult Error()
        {
            // TODO: Error handling view
            return View();
        }
    }
}

