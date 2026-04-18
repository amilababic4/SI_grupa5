using Microsoft.AspNetCore.Mvc;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Kategorije modul — CRUD kategorija (MVC)
    /// </summary>
    public class KategorijaController : Controller
    {
        // TODO: Inject IKategorijaRepository

        public async Task<IActionResult> Index()
        {
            // TODO: Pregled kategorija
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(/* TODO: KategorijaViewModel */)
        {
            // TODO: Dodavanje kategorije
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id /*, TODO: KategorijaViewModel */)
        {
            // TODO: Uređivanje kategorije
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // TODO: Brisanje (zabrana ako ima knjige)
            return RedirectToAction("Index");
        }
    }
}
