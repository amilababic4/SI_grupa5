using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Katalog modul — Upravljanje kategorijama knjiga
    /// US-30: Dodavanje | US-31: Pregled | US-32: Zabrana brisanja | US-33: Uređivanje | US-34: Brisanje
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Bibliotekar,Administrator")]
    public class KategorijaController : ControllerBase
    {
        private readonly IKategorijaRepository _kategorijaRepository;

        public KategorijaController(IKategorijaRepository kategorijaRepository)
        {
            _kategorijaRepository = kategorijaRepository;
        }

        // US-31: Pregled svih kategorija
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var kategorije = await _kategorijaRepository.GetAllAsync();

            var result = kategorije.Select(k => new
            {
                k.Id,
                k.Naziv,
                k.Opis,
                BrojKnjiga = k.Knjige.Count
            });

            return Ok(result);
        }

        // US-31: Pregled jedne kategorije
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var k = await _kategorijaRepository.GetByIdAsync(id);
            if (k == null)
                return NotFound(new { message = $"Kategorija sa ID {id} nije pronađena." });

            return Ok(new
            {
                k.Id,
                k.Naziv,
                k.Opis,
                BrojKnjiga = k.Knjige.Count
            });
        }

        // US-30: Dodavanje nove kategorije
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KategorijaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Naziv))
                return BadRequest(new { message = "Naziv kategorije je obavezan." });

            var naziv = request.Naziv.Trim();

            // US-30 AC: Kategorija već postoji
            var sve = await _kategorijaRepository.GetAllAsync();
            if (sve.Any(k => k.Naziv.Equals(naziv, StringComparison.OrdinalIgnoreCase)))
                return Conflict(new { message = $"Kategorija \"{naziv}\" već postoji u sistemu." });

            var kategorija = await _kategorijaRepository.CreateAsync(new Kategorija
            {
                Naziv = naziv,
                Opis = string.IsNullOrWhiteSpace(request.Opis) ? null : request.Opis.Trim()
            });

            return CreatedAtAction(nameof(GetById), new { id = kategorija.Id }, new
            {
                kategorija.Id,
                kategorija.Naziv,
                kategorija.Opis,
                message = $"Kategorija \"{kategorija.Naziv}\" je uspješno dodana."
            });
        }

        // US-33: Uređivanje kategorije
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] KategorijaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Naziv))
                return BadRequest(new { message = "Naziv kategorije ne smije biti prazan." });

            var kategorija = await _kategorijaRepository.GetByIdAsync(id);
            if (kategorija == null)
                return NotFound(new { message = $"Kategorija sa ID {id} nije pronađena." });

            var naziv = request.Naziv.Trim();

            // US-33 AC: Naziv već postoji kod druge kategorije
            var sve = await _kategorijaRepository.GetAllAsync();
            if (sve.Any(k => k.Id != id && k.Naziv.Equals(naziv, StringComparison.OrdinalIgnoreCase)))
                return Conflict(new { message = $"Kategorija \"{naziv}\" već postoji u sistemu." });

            kategorija.Naziv = naziv;
            kategorija.Opis = string.IsNullOrWhiteSpace(request.Opis) ? null : request.Opis.Trim();
            await _kategorijaRepository.UpdateAsync(kategorija);

            return Ok(new
            {
                kategorija.Id,
                kategorija.Naziv,
                kategorija.Opis,
                message = $"Kategorija \"{kategorija.Naziv}\" je uspješno ažurirana."
            });
        }

        // US-34 + US-32: Brisanje kategorije (zabrana ako ima knjige)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var kategorija = await _kategorijaRepository.GetByIdAsync(id);
            if (kategorija == null)
                return NotFound(new { message = $"Kategorija sa ID {id} nije pronađena." });

            // US-32 AC: Zabrana brisanja ako ima povezane knjige
            if (await _kategorijaRepository.HasBooksAsync(id))
                return Conflict(new
                {
                    message = $"Kategorija \"{kategorija.Naziv}\" ima povezane knjige i ne može biti obrisana. " +
                              "Prvo premjestite knjige u drugu kategoriju."
                });

            await _kategorijaRepository.DeleteAsync(id);

            return Ok(new
            {
                message = $"Kategorija \"{kategorija.Naziv}\" je uspješno obrisana.",
                id = kategorija.Id
            });
        }
    }

    // DTO za kreiranje/uređivanje kategorije
    public class KategorijaRequest
    {
        public string Naziv { get; set; } = string.Empty;
        public string? Opis { get; set; }
    }
}