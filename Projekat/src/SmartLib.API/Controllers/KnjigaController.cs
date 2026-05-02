using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class KnjigaController : ControllerBase
    {
        private readonly IKnjigaRepository _knjigaRepository;
        private readonly IPrimjerakRepository _primjerakRepository;
        private readonly IKategorijaRepository _kategorijaRepository;

        public KnjigaController(
            IKnjigaRepository knjigaRepository,
            IPrimjerakRepository primjerakRepository,
            IKategorijaRepository kategorijaRepository)
        {
            _knjigaRepository = knjigaRepository;
            _primjerakRepository = primjerakRepository;
            _kategorijaRepository = kategorijaRepository;
        }

        // GET: api/Knjiga
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KnjigaDto>>> GetAll([FromQuery] string? naslov, [FromQuery] string? autor, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (knjige, ukupno) = await _knjigaRepository.GetPagedAsync(naslov, autor, page, pageSize);

            var dtos = knjige.Select(MapToDto).ToList();
            
            return Ok(new
            {
                Podaci = dtos,
                UkupnoStavki = ukupno,
                Stranica = page,
                UkupnoStranica = (int)Math.Ceiling((double)ukupno / pageSize)
            });
        }

        // GET: api/Knjiga/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<KnjigaDto>> GetById(int id)
        {
            var knjiga = await _knjigaRepository.GetByIdAsync(id);
            if (knjiga == null) return NotFound();

            return Ok(MapToDto(knjiga));
        }

        // POST: api/Knjiga
        [HttpPost]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<ActionResult<KnjigaDto>> Create(KnjigaCreateDto model)
        {
            // 1. Validacija ISBN-a 
            var normalizedIsbn = NormalizeIsbn(model.Isbn);
            if (!IsValidIsbn(normalizedIsbn))
                return BadRequest("ISBN nije u ispravnom formatu.");

            var existing = await _knjigaRepository.GetByIsbnAsync(normalizedIsbn);
            if (existing != null)
                return Conflict("Knjiga sa ovim ISBN-om već postoji.");

            // 2. Provjera kategorije
            var kategorija = await _kategorijaRepository.GetByIdAsync(model.KategorijaId);
            if (kategorija == null)
                return BadRequest("Odabrana kategorija nije validna.");

            // 3. Spasavanje knjige
            var knjiga = new Knjiga
            {
                Naslov = model.Naslov.Trim(),
                Autor = model.Autor.Trim(),
                Isbn = normalizedIsbn,
                KategorijaId = model.KategorijaId,
                Izdavac = model.Izdavac?.Trim(),
                GodinaIzdanja = model.GodinaIzdanja
            };

            var savedKnjiga = await _knjigaRepository.CreateAsync(knjiga);

            // 4. Automatsko generisanje primjeraka
            for (int i = 0; i < model.BrojPrimjeraka; i++)
            {
                await _primjerakRepository.CreateAsync(new Primjerak
                {
                    KnjigaId = savedKnjiga.Id,
                    InventarniBroj = $"INV-{savedKnjiga.Id}-{i + 1:D3}",
                    Status = "dostupan",
                    DatumNabave = DateTime.UtcNow
                });
            }
            
            var result = await _knjigaRepository.GetByIdAsync(savedKnjiga.Id);
            return CreatedAtAction(nameof(GetById), new { id = savedKnjiga.Id }, MapToDto(result!));
        }

        // PUT: api/Knjiga/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<IActionResult> Update(int id, KnjigaEditDto model)
        {
            if (id != model.Id) return BadRequest("ID se ne podudara.");

            var knjiga = await _knjigaRepository.GetByIdAsync(id);
            if (knjiga == null) return NotFound();

            knjiga.Naslov = model.Naslov.Trim();
            knjiga.Autor = model.Autor.Trim();
            knjiga.KategorijaId = model.KategorijaId;
            knjiga.Izdavac = model.Izdavac?.Trim();
            knjiga.GodinaIzdanja = model.GodinaIzdanja;

            await _knjigaRepository.UpdateAsync(knjiga);
            return NoContent();
        }

        // DELETE: api/Knjiga/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Bibliotekar,Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            // 1. Provjera zaduženja (US-28)
            if (await _knjigaRepository.HasActiveLoansAsync(id))
            {
                return BadRequest(new { poruka = "Knjiga ima aktivna zaduženja i ne može biti obrisana." });
            }

            // 2. Izvršavanje brisanja
            var success = await _knjigaRepository.DeleteAsync(id);

            if (!success)
            {
                return NotFound(new { poruka = "Knjiga sa tim ID-om nije pronađena." }); // US-27
            }

            // 204 No Content je standard za uspješno brisanje u API-ju
            return NoContent();
        }

        #region Helperi (Isti kao u Web-u)

        private static KnjigaDto MapToDto(Knjiga k) => new KnjigaDto
        {
            Id = k.Id,
            Naslov = k.Naslov,
            Autor = k.Autor,
            Isbn = k.Isbn,
            Kategorija = k.Kategorija?.Naziv,
            Izdavac = k.Izdavac,
            GodinaIzdanja = k.GodinaIzdanja,
            BrojPrimjeraka = k.Primjerci.Count,
            BrojDostupnih = k.Primjerci.Count(p => p.Status == "dostupan")
        };

        private static string NormalizeIsbn(string isbn) => isbn.Replace("-", "").Replace(" ", "").Trim();

        private static bool IsValidIsbn(string isbn)
        {
            if (string.IsNullOrEmpty(isbn)) return false;
            if (isbn.Length == 13) return isbn.All(char.IsDigit);
            if (isbn.Length == 10) return isbn[..9].All(char.IsDigit) && (char.IsDigit(isbn[9]) || isbn[9] == 'X');
            return false;
        }

        #endregion
    }
}