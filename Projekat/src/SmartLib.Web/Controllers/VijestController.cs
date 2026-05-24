using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.Web.Controllers
{
    public class VijestController : Controller
    {
        private readonly IVijestRepository _repo;

        public VijestController(IVijestRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var vijesti = await _repo.GetAllAsync();
            return View(vijesti);
        }

        [Authorize(Roles = "Bibliotekar,Administrator")]
        public IActionResult Create()
        {
            return View("Upsert", new Vijest { DatumObjave = DateTime.Today });
        }

        [HttpPost]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vijest vijest)
        {
            bool isAjax = Request.Headers.ContainsKey("X-Requested-With");

            if (string.IsNullOrWhiteSpace(vijest.Naslov))
            {
                if (isAjax) return BadRequest(new { error = "Naslov je obavezan." });
                ModelState.AddModelError(nameof(vijest.Naslov), "Naslov je obavezan.");
                return View("Upsert", vijest);
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Forbid();

            vijest.AutorId = userId;
            if (vijest.DatumObjave == default)
                vijest.DatumObjave = DateTime.UtcNow;
            await _repo.CreateAsync(vijest);

            if (isAjax) return Json(new { success = true });
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var vijest = await _repo.GetByIdAsync(id);
            if (vijest == null) return NotFound();
            return View("Upsert", vijest);
        }

        [HttpPost]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vijest vijest)
        {
            bool isAjax = Request.Headers.ContainsKey("X-Requested-With");

            if (id != vijest.Id) return BadRequest();

            if (string.IsNullOrWhiteSpace(vijest.Naslov))
            {
                if (isAjax) return BadRequest(new { error = "Naslov je obavezan." });
                ModelState.AddModelError(nameof(vijest.Naslov), "Naslov je obavezan.");
                return View("Upsert", vijest);
            }

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Naslov = vijest.Naslov;
            existing.Sadrzaj = vijest.Sadrzaj;
            existing.Kategorija = vijest.Kategorija;
            existing.SlikaUrl = vijest.SlikaUrl;
            await _repo.UpdateAsync(existing);

            if (isAjax) return Json(new { success = true });
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            bool isAjax = Request.Headers.ContainsKey("X-Requested-With");
            await _repo.DeleteAsync(id);
            if (isAjax) return Json(new { success = true });
            return RedirectToAction(nameof(Index));
        }
    }
}
