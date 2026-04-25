using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.Models;

namespace SmartLib.Web.Controllers
{
    [Authorize]
    public class ZaduzenjeController : Controller
    {
        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Moja()
        {
            return View();
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        public async Task<IActionResult> Historija(int korisnikId)
        {
            return View();
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        public async Task<IActionResult> Zaduzi()
        {
            return RedirectToAction("Index");
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        public async Task<IActionResult> Vrati(int id)
        {
            return RedirectToAction("Index");
        }
    }
}