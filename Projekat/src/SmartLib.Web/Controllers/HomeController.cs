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

        public HomeController(IKnjigaRepository knjigaRepository)
        {
            _knjigaRepository = knjigaRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Dohvatamo nasumične 4 knjige za landing page
            var knjige = await _knjigaRepository.GetRandomAsync(4);
            
            var dtos = knjige.Select(k => new KnjigaDto
            {
                Id = k.Id,
                Naslov = k.Naslov,
                Autor = k.Autor,
                Isbn = k.Isbn,
                Kategorija = k.Kategorija?.Naziv,
                GodinaIzdanja = k.GodinaIzdanja
            }).ToList();

            return View(dtos);
        }


        public IActionResult Error()
        {
            // TODO: Error handling view
            return View();
        }
    }
}

