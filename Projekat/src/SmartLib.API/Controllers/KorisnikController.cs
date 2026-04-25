using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Korisnici modul — Upravljanje korisničkim nalozima
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Bibliotekar,Administrator")]
    public class KorisnikController : ControllerBase
    {
        private readonly IKorisnikRepository _korisnikRepository;

        public KorisnikController(IKorisnikRepository korisnikRepository)
        {
            _korisnikRepository = korisnikRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KorisnikDto>>> GetAll()
        {
            var korisnici = await _korisnikRepository.GetAllAsync();
            var result = korisnici.Select(MapToDto).ToList();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KorisnikDto>> GetById(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik is null)
            {
                return NotFound();
            }

            return Ok(MapToDto(korisnik));
        }

        [HttpPost]
        public async Task<ActionResult<KorisnikDto>> Create([FromBody] KorisnikCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var existingUser = await _korisnikRepository.GetByEmailAsync(model.Email);
            if (existingUser is not null)
            {
                ModelState.AddModelError(nameof(model.Email), "Ta email adresa je već registrovana.");
                return ValidationProblem(ModelState);
            }

            var korisnik = new Korisnik
            {
                Ime = model.Ime.Trim(),
                Prezime = model.Prezime.Trim(),
                Email = model.Email.Trim().ToLowerInvariant(),
                LozinkaHash = PasswordHasher.HashPassword(model.Lozinka),
                UlogaId = 1,
                Status = "aktivan",
                DatumKreiranja = DateTime.UtcNow
            };

            var created = await _korisnikRepository.CreateAsync(korisnik);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
        }

        [HttpPut("{id}/uloga")]
        public IActionResult ChangeRole(int id /*, TODO: ChangeRoleDto */)
        {
            // TODO: Promjena uloge (samo admin)
            return Ok(new { message = "TODO" });
        }

        [HttpPut("{id}/deaktiviraj")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik is null)
            {
                return NotFound();
            }

            korisnik.Status = "deaktiviran";
            await _korisnikRepository.UpdateAsync(korisnik);

            return NoContent();
        }

        private static KorisnikDto MapToDto(Korisnik korisnik)
        {
            return new KorisnikDto
            {
                Id = korisnik.Id,
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Email = korisnik.Email,
                Uloga = korisnik.Uloga?.Naziv ?? string.Empty,
                Status = korisnik.Status,
                DatumKreiranja = korisnik.DatumKreiranja
            };
        }
    }
}
