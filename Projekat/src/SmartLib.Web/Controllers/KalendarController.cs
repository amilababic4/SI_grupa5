using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.Web.Controllers
{
    public class KalendarController : Controller
    {
        private readonly IDogadjajRepository _repo;

        public KalendarController(IDogadjajRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var dogadjaji = await _repo.GetAllAsync();
            return View(dogadjaji);
        }

        [Authorize(Roles = "Bibliotekar,Administrator")]
        public IActionResult Create()
        {
            return View("Upsert", new Dogadjaj { Datum = DateTime.Today });
        }

        [HttpPost]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dogadjaj dogadjaj)
        {
            bool isAjax = Request.Headers.ContainsKey("X-Requested-With");

            if (string.IsNullOrWhiteSpace(dogadjaj.Naslov))
            {
                if (isAjax) return BadRequest(new { error = "Naziv je obavezan." });
                ModelState.AddModelError(nameof(dogadjaj.Naslov), "Naziv je obavezan.");
                return View("Upsert", dogadjaj);
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Forbid();

            dogadjaj.AutorId = userId;
            if (dogadjaj.Datum == default)
                dogadjaj.Datum = DateTime.Today;
            await _repo.CreateAsync(dogadjaj);

            if (isAjax) return Json(new { success = true, id = dogadjaj.Id, datum = dogadjaj.Datum.ToString("yyyy-MM-dd") });
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var dogadjaj = await _repo.GetByIdAsync(id);
            if (dogadjaj == null) return NotFound();
            return View("Upsert", dogadjaj);
        }

        [HttpPost]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Dogadjaj dogadjaj)
        {
            bool isAjax = Request.Headers.ContainsKey("X-Requested-With");

            if (id != dogadjaj.Id) return BadRequest();

            if (string.IsNullOrWhiteSpace(dogadjaj.Naslov))
            {
                if (isAjax) return BadRequest(new { error = "Naziv je obavezan." });
                ModelState.AddModelError(nameof(dogadjaj.Naslov), "Naziv je obavezan.");
                return View("Upsert", dogadjaj);
            }

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Naslov    = dogadjaj.Naslov;
            existing.Opis      = dogadjaj.Opis;
            existing.Datum     = dogadjaj.Datum;
            existing.Sat       = dogadjaj.Sat;
            existing.Lokacija  = dogadjaj.Lokacija;
            existing.Kategorija = dogadjaj.Kategorija;
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
