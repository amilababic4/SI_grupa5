using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Rezervacije modul — Evidencija rezervacija nedostupnih knjiga
    /// US-69, US-70, US-71, US-72, US-73, US-79, US-80
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RezervacijaController : ControllerBase
    {
        private readonly IRezervacijaRepository _rezervacijaRepo;
        private readonly IKnjigaRepository _knjigaRepo;

        public RezervacijaController(
            IRezervacijaRepository rezervacijaRepo,
            IKnjigaRepository knjigaRepo)
        {
            _rezervacijaRepo = rezervacijaRepo;
            _knjigaRepo = knjigaRepo;
        }

        // US-73: Pregled svih aktivnih rezervacija (bibliotekar)
        [HttpGet]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<IActionResult> GetActive()
        {
            var sve = await _rezervacijaRepo.GetActiveAsync();
            return Ok(sve.Select(r => new
            {
                id = r.Id,
                korisnikIme = r.Korisnik != null ? $"{r.Korisnik.Ime} {r.Korisnik.Prezime}" : "-",
                korisnikEmail = r.Korisnik?.Email ?? "-",
                knjigaNaslov = r.Knjiga?.Naslov ?? "-",
                datumRezervacije = r.DatumRezervacije,
                datumIsteka = r.DatumIsteka,
                status = r.Status
            }));
        }

        // US-71: Pregled vlastitih aktivnih rezervacija (prijavljeni član)
        [HttpGet("moje")]
        public async Task<IActionResult> GetMine()
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return Unauthorized(new { poruka = "Korisnik nije identificiran." });

            var rezervacije = await _rezervacijaRepo.GetByKorisnikAsync(korisnikId);
            return Ok(rezervacije.Select(r => new
            {
                id = r.Id,
                knjigaNaslov = r.Knjiga?.Naslov ?? "-",
                datumRezervacije = r.DatumRezervacije,
                datumIsteka = r.DatumIsteka,
                status = r.Status
            }));
        }

        // US-69, US-70, US-79: Kreiranje rezervacije za nedostupnu knjigu
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RezervacijaCreateDto dto)
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return Unauthorized(new { poruka = "Korisnik nije identificiran." });

            var knjiga = await _knjigaRepo.GetByIdAsync(dto.KnjigaId);
            if (knjiga == null)
                return NotFound(new { poruka = "Knjiga nije pronađena." });

            // US-69: Rezervacija dozvoljena samo kad nema dostupnih primjeraka
            bool imaDostupnih = knjiga.Primjerci.Any(p => p.Status == "dostupan");
            if (imaDostupnih)
                return BadRequest(new { poruka = "Rezervacija nije moguća — knjiga ima dostupnih primjeraka." });

            // US-69: Jedan član ne može imati dvije aktivne rezervacije iste knjige
            bool imaDuplikat = await _rezervacijaRepo.HasActiveAsync(korisnikId, dto.KnjigaId);
            if (imaDuplikat)
                return Conflict(new { poruka = "Već imate aktivnu rezervaciju za ovu knjigu." });

            var rezervacija = new Rezervacija
            {
                KorisnikId = korisnikId,
                KnjigaId = dto.KnjigaId,
                DatumRezervacije = DateTime.UtcNow,
                DatumIsteka = DateTime.UtcNow.AddDays(7),
                Status = "aktivna"
            };

            await _rezervacijaRepo.CreateAsync(rezervacija);
            return StatusCode(201, new { id = rezervacija.Id, poruka = "Rezervacija je uspješno kreirana." });
        }

        // US-72, US-80: Otkazivanje vlastite rezervacije
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return Unauthorized(new { poruka = "Korisnik nije identificiran." });

            var rezervacija = await _rezervacijaRepo.GetByIdAsync(id);
            if (rezervacija == null)
                return NotFound(new { poruka = "Rezervacija nije pronađena." });

            // Samo vlasnik može otkazati svoju rezervaciju
            if (rezervacija.KorisnikId != korisnikId)
                return Forbid();

            if (rezervacija.Status != "aktivna")
                return BadRequest(new { poruka = "Rezervacija nije aktivna." });

            rezervacija.Status = "otkazana";
            await _rezervacijaRepo.UpdateAsync(rezervacija);
            return Ok(new { poruka = "Rezervacija je uspješno otkazana." });
        }
    }
}
