// Lokacija: SmartLib.Web/Controllers/ClanarinaController.cs
// ZAMIJENI cijeli postojeći fajl ovim sadržajem.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.Web.Controllers
{
    [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
    public class ClanarinaController : Controller
    {
        private readonly IClanarinaRepository _clanarinaRepo;

        public ClanarinaController(IClanarinaRepository clanarinaRepo)
        {
            _clanarinaRepo = clanarinaRepo;
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET: /Clanarina/Upsert?korisnikId=5
        // Otvara formu za dodavanje NOVE ili izmjenu POSTOJEĆE članarine.
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Upsert(int korisnikId)
        {
            var postojeca = await _clanarinaRepo.GetByKorisnikAsync(korisnikId);

            var dto = postojeca is not null
                ? new ClanarinaUpsertDto
                {
                    KorisnikId = korisnikId,
                    DatumPocetka = postojeca.DatumPocetka,
                    DatumIsteka = postojeca.DatumIsteka
                }
                : new ClanarinaUpsertDto
                {
                    KorisnikId = korisnikId,
                    DatumPocetka = DateTime.Today,
                    DatumIsteka = DateTime.Today.AddYears(1)
                };

            // Proslijedi bool kako bi View znao prikazati "Nova" ili "Uredi" naslov
            ViewBag.Postoji = postojeca is not null;
            return View(dto);
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST: /Clanarina/Upsert
        // Sprema novu ili ažurira postojeću članarinu.
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ClanarinaUpsertDto dto)
        {
            // Validacija: datum isteka mora biti nakon datuma početka
            if (dto.DatumIsteka <= dto.DatumPocetka)
                ModelState.AddModelError(nameof(dto.DatumIsteka),
                    "Datum isteka mora biti nakon datuma početka.");

            if (!ModelState.IsValid)
            {
                ViewBag.Postoji = await _clanarinaRepo.GetByKorisnikAsync(dto.KorisnikId) is not null;
                return View(dto);
            }

            var postojeca = await _clanarinaRepo.GetByKorisnikAsync(dto.KorisnikId);

            if (postojeca is null)
            {
                // US-56: Evidentiraj novu članarinu
                await _clanarinaRepo.CreateAsync(new Clanarina
                {
                    KorisnikId = dto.KorisnikId,
                    DatumPocetka = dto.DatumPocetka,
                    DatumIsteka = dto.DatumIsteka
                });
            }
            else
            {
                // US-57: Ažuriraj / produži postojeću
                postojeca.DatumPocetka = dto.DatumPocetka;
                postojeca.DatumIsteka = dto.DatumIsteka;
                await _clanarinaRepo.UpdateAsync(postojeca);
            }

            TempData["Uspjeh"] = "Članarina je uspješno spremljena.";
            return RedirectToAction("ProfilClana", "Korisnik", new { id = dto.KorisnikId });
        }
    }
}