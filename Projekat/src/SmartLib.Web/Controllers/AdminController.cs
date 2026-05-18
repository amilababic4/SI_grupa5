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

        public Task<IActionResult> Korisnici()
        {
            // TODO: Pregled svih korisnika (samo admin)
            return Task.FromResult<IActionResult>(View());
        }

        public Task<IActionResult> AuditLog(int page = 1)
        {
            // TODO: Pregled audit log zapisa (samo admin)
            return Task.FromResult<IActionResult>(View());
        }

        [HttpPost]
        public Task<IActionResult> PromijeniUlogu(int id /*, TODO: string novaUloga */)
        {
            // TODO: Promjena uloge korisnika
            return Task.FromResult<IActionResult>(RedirectToAction("Korisnici"));
        }

        [HttpPost]
        public Task<IActionResult> DeaktivirajKorisnika(int id)
        {
            // TODO: Deaktivacija bilo kojeg korisnika
            return Task.FromResult<IActionResult>(RedirectToAction("Korisnici"));
        }
    }
}
