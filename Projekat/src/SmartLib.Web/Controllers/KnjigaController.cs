using Microsoft.AspNetCore.Mvc;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Katalog modul — Pregled, pretraga, CRUD knjiga (MVC)
    /// </summary>
    public class KnjigaController : Controller
    {
        // TODO: Inject IKnjigaRepository, IKategorijaRepository, IPrimjerakRepository

        public async Task<IActionResult> Index(string? naslov, string? autor, int page = 1)
        {
            // TODO: Pregled kataloga sa pretragom i paginacijom
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            // TODO: Detalji knjige sa primjercima
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            // TODO: Forma za dodavanje knjige (bibliotekar/admin)
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // TODO: Forma za uređivanje knjige
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // TODO: Brisanje knjige (provjera aktivnih zaduženja)
            return RedirectToAction("Index");
        }
    }
}
