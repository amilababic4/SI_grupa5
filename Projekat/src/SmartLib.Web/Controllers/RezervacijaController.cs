using Microsoft.AspNetCore.Mvc;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Rezervacije modul — Kreiranje i otkazivanje rezervacija (MVC)
    /// </summary>
    public class RezervacijaController : Controller
    {
        // TODO: Inject IRezervacijaRepository

        public async Task<IActionResult> Index()
        {
            // TODO: Pregled aktivnih rezervacija (bibliotekar)
            return View();
        }

        public async Task<IActionResult> Moje()
        {
            // TODO: Pregled vlastitih rezervacija (član)
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int knjigaId)
        {
            // TODO: Kreiranje rezervacije
            return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
        }

        [HttpPost]
        public async Task<IActionResult> Otkazi(int id)
        {
            // TODO: Otkazivanje rezervacije
            return RedirectToAction("Moje");
        }
    }
}
