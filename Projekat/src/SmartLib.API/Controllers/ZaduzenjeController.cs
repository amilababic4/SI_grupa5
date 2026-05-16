using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Zaduženja modul — Evidencija zaduživanja i vraćanja knjiga
    /// US-44, US-45, US-46, US-47, US-62, US-63, US-64, US-65, US-66, US-67, US-68
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ZaduzenjeController : ControllerBase
    {
        private readonly IZaduzenjeRepository _zaduzenjeRepo;
        private readonly IKorisnikRepository _korisnikRepo;
        private readonly IKnjigaRepository _knjigaRepo;
        private readonly IPrimjerakRepository _primjerakRepo;

        public ZaduzenjeController(
            IZaduzenjeRepository zaduzenjeRepo,
            IKorisnikRepository korisnikRepo,
            IKnjigaRepository knjigaRepo,
            IPrimjerakRepository primjerakRepo)
        {
            _zaduzenjeRepo = zaduzenjeRepo;
            _korisnikRepo = korisnikRepo;
            _knjigaRepo = knjigaRepo;
            _primjerakRepo = primjerakRepo;
        }

        [HttpGet]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<IActionResult> GetActive([FromQuery] string? clan)
        {
            var sva = await _zaduzenjeRepo.GetActiveAsync();

            if (!string.IsNullOrWhiteSpace(clan))
            {
                var filter = clan.Trim().ToLowerInvariant();
                sva = sva.Where(z =>
                    ($"{z.Korisnik?.Ime} {z.Korisnik?.Prezime}").ToLowerInvariant().Contains(filter) ||
                    (z.Korisnik?.Email ?? string.Empty).ToLowerInvariant().Contains(filter));
            }

            return Ok(sva.Select(MapToDto));
        }

        // US-62, US-63, US-64: Vlastita zaduženja prijavljenog korisnika
        [HttpGet("moja")]
        public async Task<IActionResult> GetMine()
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return Unauthorized(new { poruka = "Korisnik nije identificiran." });

            var zaduzenja = await _zaduzenjeRepo.GetByKorisnikAsync(korisnikId);
            return Ok(zaduzenja.Select(MapToDto));
        }

        // US-67: Detalji jednog zaduženja (bibliotekar)
        [HttpGet("{id}")]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<IActionResult> GetById(int id)
        {
            var z = await _zaduzenjeRepo.GetByIdAsync(id);
            if (z == null) return NotFound(new { poruka = "Zaduženje nije pronađeno." });

            return Ok(MapToDto(z));
        }

        // Historija zaduženja člana (bibliotekar)
        [HttpGet("historija/{korisnikId}")]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<IActionResult> GetHistory(int korisnikId)
        {
            var korisnik = await _korisnikRepo.GetByIdAsync(korisnikId);
            if (korisnik == null)
                return NotFound(new { poruka = "Korisnik nije pronađen." });

            var zaduzenja = await _zaduzenjeRepo.GetHistoryByKorisnikAsync(korisnikId);
            return Ok(zaduzenja.Select(MapToDto));
        }

        // US-44, US-46, US-47: Kreiranje zaduženja (bibliotekar)
        [HttpPost("zaduzi")]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<IActionResult> Zaduzi([FromBody] ZaduzenjeCreateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // US-47: Provjera dostupnosti primjerka
            var primjerak = await _primjerakRepo.GetByIdAsync(model.PrimjerakId);
            if (primjerak == null || primjerak.Status != "dostupan")
                return BadRequest(new { poruka = "Odabrani primjerak nije dostupan za zaduživanje." });

            // US-47: Provjera duplikata aktivnog zaduženja
            if (await _primjerakRepo.HasActiveZaduzenjeAsync(model.PrimjerakId))
                return BadRequest(new { poruka = "Odabrani primjerak već ima aktivno zaduženje." });

            // Provjera da rok vraćanja nije u prošlosti
            if (model.DatumPovratka.HasValue && model.DatumPovratka.Value.Date < DateTime.UtcNow.Date)
                return BadRequest(new { poruka = "Datum povratka ne može biti u prošlosti." });

            var datumZaduzivanja = DateTime.UtcNow;
            var datumPlaniranogVracanja = model.DatumPovratka.HasValue
                ? DateTime.SpecifyKind(model.DatumPovratka.Value.Date, DateTimeKind.Utc)
                : datumZaduzivanja.AddMonths(2);

            var zaduzenje = new Zaduzenje
            {
                KorisnikId = model.KorisnikId,
                PrimjerakId = model.PrimjerakId,
                DatumZaduzivanja = datumZaduzivanja,
                DatumPlaniranogVracanja = datumPlaniranogVracanja,
                Status = "aktivno"
            };

            await _zaduzenjeRepo.CreateAsync(zaduzenje);
            await _primjerakRepo.UpdateStatusAsync(model.PrimjerakId, "zadužen");

            var kreirano = await _zaduzenjeRepo.GetByIdAsync(zaduzenje.Id);
            return CreatedAtAction(nameof(GetById), new { id = zaduzenje.Id }, MapToDto(kreirano!));
        }

        // US-45: Vraćanje knjige (bibliotekar)
        [HttpPost("vrati/{id}")]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<IActionResult> Vrati(int id)
        {
            var z = await _zaduzenjeRepo.GetByIdAsync(id);
            if (z == null)
                return NotFound(new { poruka = "Zaduženje nije pronađeno." });

            if (z.Status != "aktivno")
                return BadRequest(new { poruka = "Zaduženje nije aktivno." });

            z.Status = "zatvoreno";
            z.DatumStvarnogVracanja = DateTime.UtcNow;
            await _zaduzenjeRepo.UpdateAsync(z);
            await _primjerakRepo.UpdateStatusAsync(z.PrimjerakId, "dostupan");

            return Ok(MapToDto(z));
        }

        /// US-54: Evidencija vraćanja knjige (bibliotekar)
        [HttpGet("historija")]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<IActionResult> Historija([FromQuery] string? clan)
        {
            var granica = DateTime.UtcNow.AddYears(-3);
            var sva = await _zaduzenjeRepo.GetClosedSinceAsync(granica);

            if (!string.IsNullOrWhiteSpace(clan))
            {
                var filter = clan.Trim().ToLowerInvariant();
                sva = sva.Where(z =>
                    ($"{z.Korisnik?.Ime} {z.Korisnik?.Prezime}").ToLowerInvariant().Contains(filter) ||
                    (z.Korisnik?.Email ?? string.Empty).ToLowerInvariant().Contains(filter));
            }

            return Ok(sva.Select(MapToDto));
        }

        // Nema user story, dodao jer mi je bilo logično 
        [HttpGet("moja/historija")]
        public async Task<IActionResult> MojaHistorija()
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return Unauthorized(new { poruka = "Korisnik nije identificiran." });

            var granica = DateTime.UtcNow.AddYears(-3);
            var sva = await _zaduzenjeRepo.GetByKorisnikAsync(korisnikId);

            var historija = sva
                .Where(z => z.Status == "zatvoreno" && z.DatumStvarnogVracanja >= granica)
                .Select(MapToDto);

            return Ok(historija);
        }

        // Nema user story, dodao jer mi je bilo logično 
        [Authorize]
        public async Task<IActionResult> MojaHistorija()
        {
            var korisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(korisnikIdStr, out var korisnikId))
                return RedirectToAction("Login", "Auth");

            var granica = DateTime.UtcNow.AddYears(-3);

            var mojaZaduzenja = await _zaduzenjeRepo.GetClosedHistoryForKorisnikAsync(korisnikId, granica);

            var model = mojaZaduzenja.Select(MapToViewModel).ToList();

            return View(model);
        }

        #region Helperi

        private static object MapToDto(Zaduzenje z)
        {
            var now = DateTime.UtcNow;
            var zakasnilo = z.Status == "aktivno" && z.DatumPlaniranogVracanja < now;
            return new
            {
                z.Id,
                KorisnikIme = z.Korisnik != null ? $"{z.Korisnik.Ime} {z.Korisnik.Prezime}" : "-",
                KorisnikEmail = z.Korisnik?.Email ?? "-",
                KnjigaNaslov = z.Primjerak?.Knjiga?.Naslov ?? "-",
                InventarniBroj = z.Primjerak?.InventarniBroj ?? "-",
                z.DatumZaduzivanja,
                z.DatumPlaniranogVracanja,
                z.DatumStvarnogVracanja,
                z.Status,
                JeZakasnilo = zakasnilo,
                RokSeBliži = !zakasnilo && z.Status == "aktivno" && z.DatumPlaniranogVracanja <= now.AddDays(3)
            };
        }

        #endregion
    }
}