using Microsoft.AspNetCore.Mvc;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Korisnici modul — Pregled članova, profil (MVC)
    /// </summary>
    public class KorisnikController : Controller
    {
        // TODO: Inject IKorisnikRepository, IClanarinaRepository

        public async Task<IActionResult> Index()
        {
            // TODO: Pregled članova biblioteke (bibliotekar/admin)
            return View();
        }

        public async Task<IActionResult> Profil()
        {
            // TODO: Pregled vlastitog profila sa zaduženjima i članarinom
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            // TODO: Forma za kreiranje novog člana
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(/* TODO: KorisnikCreateViewModel */)
        {
            // TODO: Kreiranje novog člana
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Deaktiviraj(int id)
        {
            // TODO: Deaktivacija naloga člana (soft delete)
            return RedirectToAction("Index");
        }
    }
}
