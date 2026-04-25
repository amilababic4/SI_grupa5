using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.Models;

namespace SmartLib.Web.Controllers
{
    [Authorize]
    public class KnjigaController : Controller
    {
        public async Task<IActionResult> Index(string? naslov, string? autor, int page = 1)
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            return View();
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            return View();
        }

        [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            return RedirectToAction("Index");
        }
    }
}