using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartLib.Core.Models;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Članarina modul — Upravljanje članarinama (MVC)
    /// </summary>
    [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
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
