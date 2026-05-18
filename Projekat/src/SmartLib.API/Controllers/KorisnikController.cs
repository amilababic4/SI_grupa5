using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Security;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SmartLib.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KorisnikController : ControllerBase
    {
        private readonly IKorisnikRepository _korisnikRepository;

        public KorisnikController(IKorisnikRepository korisnikRepository)
        {
            _korisnikRepository = korisnikRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<ActionResult<IEnumerable<KorisnikDto>>> GetAll()
        {
            var korisnici = await _korisnikRepository.GetAllAsync();
            var result = korisnici
                .Where(k => string.Equals(k.Status, "aktivan", StringComparison.OrdinalIgnoreCase))
                .Select(MapToDto)
                .ToList();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<ActionResult<KorisnikDto>> GetById(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik is null)
                return NotFound();

            return Ok(MapToDto(korisnik));
        }

        [HttpGet("profil")]
        [Authorize]
        public async Task<IActionResult> Profil()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim, out int id))
                return Unauthorized(new { poruka = "Korisnik nije identificiran." });

            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik == null)
                return NotFound(new { poruka = "Korisnik nije pronađen." });

            return Ok(MapToDto(korisnik));
        }

        [HttpGet("{id}/profil")]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<IActionResult> ProfilClana(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik == null)
                return NotFound(new { poruka = "Korisnik nije pronađen." });

            return Ok(MapToDto(korisnik));
        }

        [HttpPost]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<ActionResult<KorisnikDto>> Create([FromBody] KorisnikCreateDto model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (model.Lozinka != model.PotvrdaLozinke)
            {
                ModelState.AddModelError(nameof(model.PotvrdaLozinke), "Lozinka i potvrda lozinke se ne poklapaju.");
                return ValidationProblem(ModelState);
            }

            // ✅ XSS zaštita
            if (SadrziHtml(model.Ime) || SadrziHtml(model.Prezime))
                return BadRequest(new { message = "Ime/prezime ne smije sadržavati HTML tagove." });

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
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public IActionResult ChangeRole(int id)
        {
            return Ok(new { message = "TODO" });
        }

        [HttpPut("{id}/deaktiviraj")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik is null)
                return NotFound();

            var requestUserId = GetUserId();
            var isSelf = requestUserId.HasValue && requestUserId.Value == id;
            var isStaff = IsStaff();

            if (!isSelf && !isStaff)
                return Forbid();

            var targetIsAdmin = string.Equals(
                korisnik.Uloga?.Naziv,
                RoleNames.Administrator,
                StringComparison.OrdinalIgnoreCase);

            if (targetIsAdmin)
                return Forbid();

            korisnik.Status = "deaktiviran";
            korisnik.DatumDeaktivacije = DateTime.UtcNow;
            await _korisnikRepository.UpdateAsync(korisnik);
            return NoContent();
        }

        [HttpPut("{id}/aktiviraj")]
        public async Task<IActionResult> Reactivate(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik is null)
                return NotFound();

            var requestUserId = GetUserId();
            var isSelf = requestUserId.HasValue && requestUserId.Value == id;
            var isStaff = IsStaff();

            if (!isSelf && !isStaff)
                return Forbid();

            if (string.Equals(korisnik.Status, "aktivan", StringComparison.OrdinalIgnoreCase))
                return NoContent();

            korisnik.Status = "aktivan";
            korisnik.DatumDeaktivacije = null;
            await _korisnikRepository.UpdateAsync(korisnik);
            return NoContent();
        }

        private bool IsStaff()
        {
            return User.IsInRole(RoleNames.Bibliotekar) || User.IsInRole(RoleNames.Administrator);
        }

        private int? GetUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var id) ? id : null;
        }

        // XSS helper
        private static bool SadrziHtml(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            return Regex.IsMatch(input, "<.*?>|&[a-z]+;", RegexOptions.IgnoreCase);
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