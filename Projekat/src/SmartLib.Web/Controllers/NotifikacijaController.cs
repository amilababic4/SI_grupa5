using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.Interfaces;

namespace SmartLib.Web.Controllers
{
    [Authorize]
    public class NotifikacijaController : Controller
    {
        private readonly INotifikacijaRepository _notifikacijaRepo;

        public NotifikacijaController(INotifikacijaRepository notifikacijaRepo)
        {
            _notifikacijaRepo = notifikacijaRepo;
        }

        public async Task<IActionResult> Index()
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return Challenge();

            var notifikacije = await _notifikacijaRepo.GetForUserAsync(korisnikId);
            return View(notifikacije);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(int id)
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return Challenge();

            await _notifikacijaRepo.MarkReadAsync(id, korisnikId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead()
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return Challenge();

            await _notifikacijaRepo.MarkAllReadAsync(korisnikId);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Otvori(int id, string url)
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return Challenge();

            // Mark as read immediately when clicked
            await _notifikacijaRepo.MarkReadAsync(id, korisnikId);

            if (!string.IsNullOrWhiteSpace(url))
            {
                return Redirect(url);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
