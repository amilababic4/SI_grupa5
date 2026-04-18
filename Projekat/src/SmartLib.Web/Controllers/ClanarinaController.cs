using Microsoft.AspNetCore.Mvc;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Članarina modul — Upravljanje članarinama (MVC)
    /// </summary>
    public class ClanarinaController : Controller
    {
        // TODO: Inject IClanarinaRepository

        [HttpPost]
        public async Task<IActionResult> Create(/* TODO: ClanarinaCreateViewModel */)
        {
            // TODO: Evidentiranje nove članarine
            return RedirectToAction("Profil", "Korisnik");
        }

        [HttpPost]
        public async Task<IActionResult> Produzi(int id /*, TODO: ClanarinaUpdateViewModel */)
        {
            // TODO: Produženje članarine
            return RedirectToAction("Profil", "Korisnik");
        }
    }
}
