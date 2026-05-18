using Microsoft.AspNetCore.Mvc;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Rezervacije modul — Kreiranje i otkazivanje rezervacija (MVC)
    /// </summary>
    public class RezervacijaController : Controller
    {
        // TODO: Inject IRezervacijaRepository

        public Task<IActionResult> Index()
        {
            // TODO: Pregled aktivnih rezervacija (bibliotekar)
            return Task.FromResult<IActionResult>(View());
        }

        public Task<IActionResult> Moje()
        {
            // TODO: Pregled vlastitih rezervacija (član)
            return Task.FromResult<IActionResult>(View());
        }

        [HttpPost]
        public Task<IActionResult> Create(int knjigaId)
        {
            // TODO: Kreiranje rezervacije
            return Task.FromResult<IActionResult>(RedirectToAction("Details", "Knjiga", new { id = knjigaId }));
        }

        [HttpPost]
        public Task<IActionResult> Otkazi(int id)
        {
            // TODO: Otkazivanje rezervacije
            return Task.FromResult<IActionResult>(RedirectToAction("Moje"));
        }
    }
}
