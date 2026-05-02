using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.Web.Controllers
{
    [Authorize(Roles = "Bibliotekar,Administrator")]
    public class PrimjerakController : Controller
    {
        private readonly IPrimjerakRepository _primjerakRepository;
        private readonly IKnjigaRepository _knjigaRepository;

        public PrimjerakController(
            IPrimjerakRepository primjerakRepository,
            IKnjigaRepository knjigaRepository)
        {
            _primjerakRepository = primjerakRepository;
            _knjigaRepository = knjigaRepository;
        }

        // US-21: Dodavanje novog primjerka postojećoj knjizi (GET forma)
        [HttpGet]
        public async Task<IActionResult> Dodaj(int knjigaId)
        {
            var knjiga = await _knjigaRepository.GetByIdAsync(knjigaId);
            if (knjiga == null) return NotFound();

            ViewBag.Knjiga = knjiga;
            return View();
        }

        // US-21: Dodavanje novog primjerka (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(int knjigaId, int brojNovih)
        {
            // Primjerak mora imati povezanu knjigu
            var knjiga = await _knjigaRepository.GetByIdAsync(knjigaId);
            if (knjiga == null)
            {
                TempData["ErrorMessage"] = "Knjiga nije pronađena.";
                return RedirectToAction("Index", "Knjiga");
            }

            if (brojNovih < 1 || brojNovih > 50)
            {
                TempData["ErrorMessage"] = "Broj primjeraka mora biti između 1 i 50.";
                return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
            }

            // Dohvati trenutne primjerke da bi generisali sljedeći redni broj
            var postojeci = (await _primjerakRepository.GetByKnjigaAsync(knjigaId)).ToList();
            int sljedeciRedni = postojeci.Count + 1;

            for (int i = 0; i < brojNovih; i++)
            {
                await _primjerakRepository.CreateAsync(new Primjerak
                {
                    KnjigaId = knjigaId,
                    InventarniBroj = $"INV-{knjigaId}-{sljedeciRedni + i:D3}",
                    Status = "dostupan",
                    DatumNabave = DateTime.UtcNow
                });
            }

            TempData["SuccessMessage"] = $"Uspješno dodano {brojNovih} novi{(brojNovih == 1 ? "" : "h")} primjerak{(brojNovih == 1 ? "" : "a")}.";
            return RedirectToAction("Details", "Knjiga", new { id = knjigaId });
        }

        // US-24: Deaktivacija primjerka
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deaktiviraj(int id)
        {
            var primjerak = await _primjerakRepository.GetByIdAsync(id);
            if (primjerak == null)
            {
                TempData["ErrorMessage"] = "Primjerak nije pronađen.";
                return RedirectToAction("Index", "Knjiga");
            }

            // AC: Sistem ne dozvoljava deaktivaciju primjerka koji je trenutno aktivno zadužen
            if (await _primjerakRepository.HasActiveZaduzenjeAsync(id))
            {
                TempData["ErrorMessage"] =
                    $"Primjerak {primjerak.InventarniBroj} je trenutno aktivo zadužen i ne može biti deaktiviran.";
                return RedirectToAction("Details", "Knjiga", new { id = primjerak.KnjigaId });
            }

            if (primjerak.Status == "deaktiviran")
            {
                TempData["ErrorMessage"] = $"Primjerak {primjerak.InventarniBroj} je već deaktiviran.";
                return RedirectToAction("Details", "Knjiga", new { id = primjerak.KnjigaId });
            }

            await _primjerakRepository.DeactivateAsync(id);

            TempData["SuccessMessage"] =
                $"Primjerak {primjerak.InventarniBroj} je uspješno deaktiviran i više nije dostupan za zaduživanje.";
            return RedirectToAction("Details", "Knjiga", new { id = primjerak.KnjigaId });
        }
    }
}