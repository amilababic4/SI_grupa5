using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.Interfaces;
using SmartLib.Core.DTOs;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Početna stranica i opće rute
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IKnjigaRepository _knjigaRepository;
        private readonly IBookRecommender _bookRecommender;

        public HomeController(IKnjigaRepository knjigaRepository, IBookRecommender bookRecommender)
        {
            _knjigaRepository = knjigaRepository;
            _bookRecommender = bookRecommender;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new Models.HomeViewModel();

            // Dohvatamo nasumične 4 knjige za landing page
            var randomKnjige = await _knjigaRepository.GetRandomAsync(4);
            viewModel.RandomKnjige = randomKnjige.Select(k => new KnjigaDto
            {
                Id = k.Id,
                Naslov = k.Naslov,
                Autor = k.Autor,
                Isbn = k.Isbn,
                Kategorija = k.Kategorija?.Naziv,
                GodinaIzdanja = k.GodinaIzdanja
            }).ToList();

            // Ako je korisnik prijavljen, dohvatamo preporuke
            if (User.Identity?.IsAuthenticated ?? false)
            {
                // Za sada koristimo dummy ID 1 ili dohvatamo pravi ID iz claimova ako postoji
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    var recommended = await _bookRecommender.GetRecommendationsForUserAsync(userId);
                    viewModel.RecommendedKnjige = recommended.Select(k => new KnjigaDto
                    {
                        Id = k.Id,
                        Naslov = k.Naslov,
                        Autor = k.Autor,
                        Isbn = k.Isbn,
                        Kategorija = k.Kategorija?.Naziv,
                        GodinaIzdanja = k.GodinaIzdanja
                    }).ToList();
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

