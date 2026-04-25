using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartLib.Core.Models;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Članarina, Rezervacije, Admin panel (MVC)
    /// </summary>
    [Authorize(Roles = RoleNames.Administrator)]
    public class AdminController : Controller
    {
        // TODO: Inject potrebne repozitorije

        public async Task<IActionResult> Korisnici()
        {
            // TODO: Pregled svih korisnika (samo admin)
            return View();
        }

        public async Task<IActionResult> AuditLog(int page = 1)
        {
            // TODO: Pregled audit log zapisa (samo admin)
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PromijeniUlogu(int id /*, TODO: string novaUloga */)
        {
            // TODO: Promjena uloge korisnika
            return RedirectToAction("Korisnici");
        }

        [HttpPost]
        public async Task<IActionResult> DeaktivirajKorisnika(int id)
        {
            // TODO: Deaktivacija bilo kojeg korisnika
            return RedirectToAction("Korisnici");
        }
    }
}
