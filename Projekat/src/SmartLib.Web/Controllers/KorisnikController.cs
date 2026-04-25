using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.Web.Controllers
{
    /// <summary>
    /// Korisnici modul — Pregled članova, profil (MVC)
    /// </summary>
    [Authorize(Roles = RoleNames.Bibliotekar + "," + RoleNames.Administrator)]
    public class KorisnikController : Controller
    {
        private readonly IKorisnikRepository _korisnikRepository;

        public KorisnikController(IKorisnikRepository korisnikRepository)
        {
            _korisnikRepository = korisnikRepository;
        }

        public async Task<IActionResult> Index()
        {
            var korisnici = await _korisnikRepository.GetAllAsync();
            var clanovi = korisnici
                .Where(k => string.Equals(k.Uloga?.Naziv, RoleNames.Clan, StringComparison.OrdinalIgnoreCase))
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .Select(k => new KorisnikDto
                {
                    Id = k.Id,
                    Ime = k.Ime,
                    Prezime = k.Prezime,
                    Email = k.Email,
                    Uloga = k.Uloga?.Naziv ?? RoleNames.Clan,
                    Status = k.Status,
                    DatumKreiranja = k.DatumKreiranja
                })
                .ToList();

            return View(clanovi);
        }

        public async Task<IActionResult> Profil()
        {
            // Pregled vlastitog profila sa zaduženjima i članarinom će se dodati kasnije.
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new KorisnikCreateDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(KorisnikCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _korisnikRepository.GetByEmailAsync(model.Email);
            if (existingUser is not null)
            {
                ModelState.AddModelError(nameof(model.Email), "Ta email adresa je već registrovana.");
                return View(model);
            }

            var korisnik = new SmartLib.Core.Models.Korisnik
            {
                Ime = model.Ime.Trim(),
                Prezime = model.Prezime.Trim(),
                Email = model.Email.Trim().ToLowerInvariant(),
                LozinkaHash = SmartLib.Infrastructure.Security.PasswordHasher.HashPassword(model.Lozinka),
                UlogaId = 1,
                Status = "aktivan",
                DatumKreiranja = DateTime.UtcNow
            };

            await _korisnikRepository.CreateAsync(korisnik);

            TempData["SuccessMessage"] = "Nalog člana je uspješno kreiran.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Deaktiviraj(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik is null)
            {
                return NotFound();
            }

            korisnik.Status = "deaktiviran";
            await _korisnikRepository.UpdateAsync(korisnik);

            TempData["SuccessMessage"] = "Nalog člana je deaktiviran.";
            return RedirectToAction("Index");
        }
    }
}
