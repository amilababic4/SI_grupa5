using Microsoft.AspNetCore.Mvc;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Zaduženja modul — Zaduživanje, vraćanje, pregled (MVC)
    /// </summary>
    public class ZaduzenjeController : Controller
    {
        // TODO: Inject IZaduzenjeRepository, IPrimjerakRepository, IClanarinaRepository

        public async Task<IActionResult> Index()
        {
            // TODO: Pregled aktivnih zaduženja
            return View();
        }

        public async Task<IActionResult> Moja()
        {
            // TODO: Pregled vlastitih zaduženja (član)
            return View();
        }

        public async Task<IActionResult> Historija(int korisnikId)
        {
            // TODO: Historija zaduženja člana
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Zaduzi(/* TODO: ZaduzenjeCreateViewModel */)
        {
            // TODO: Kreiranje zaduženja (atomska transakcija)
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Vrati(int id)
        {
            // TODO: Vraćanje knjige (atomska transakcija)
            return RedirectToAction("Index");
        }
    }
}
