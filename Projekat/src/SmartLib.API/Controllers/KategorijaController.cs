using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Services;
using System.Text.RegularExpressions;

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
        private readonly IDistributedCache _cache;
        private readonly CacheVersionStore _cacheVersions;
        private static readonly TimeSpan CategoriesCacheTtl = TimeSpan.FromMinutes(5);

        public KategorijaController(
            IKategorijaRepository kategorijaRepository,
            IDistributedCache cache,
            CacheVersionStore cacheVersions)
        {
            _kategorijaRepository = kategorijaRepository;
            _cache = cache;
            _cacheVersions = cacheVersions;
        }

        // US-31: Pregled svih kategorija
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cacheKey = $"api_categories_v1_{_cacheVersions.CategoriesVersion}_{_cacheVersions.BooksVersion}";
            var result = await _cache.GetOrCreateRecordAsync(cacheKey, CategoriesCacheTtl, async () =>
            {
                var kategorije = await _kategorijaRepository.GetAllAsync();
                return kategorije.Select(k => new KategorijaResponse
                {
                    Id = k.Id,
                    Naziv = k.Naziv,
                    Opis = k.Opis,
                    BrojKnjiga = k.Knjige.Count
                }).ToList();
            });

            return Ok(result);
        }

        // US-31: Pregled jedne kategorije
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cacheKey = $"api_category_{id}_v1_{_cacheVersions.CategoriesVersion}_{_cacheVersions.BooksVersion}";
            var cached = await _cache.GetRecordAsync<KategorijaResponse>(cacheKey);
            if (cached != null)
                return Ok(cached);

            var k = await _kategorijaRepository.GetByIdAsync(id);
            if (k == null)
                return NotFound(new { message = $"Kategorija sa ID {id} nije pronađena." });

            var result = new KategorijaResponse
            {
                Id = k.Id,
                Naziv = k.Naziv,
                Opis = k.Opis,
                BrojKnjiga = k.Knjige.Count
            };

            await _cache.SetRecordAsync(cacheKey, result, CategoriesCacheTtl);
            return Ok(result);
        }

        // US-30: Dodavanje nove kategorije
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KategorijaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (SadrziHtml(request.Naziv) || SadrziHtml(request.Opis))
                return BadRequest(new { message = "Naziv/opis ne smije sadržavati HTML tagove." });

            if (string.IsNullOrWhiteSpace(request.Naziv))
                return BadRequest(new { message = "Naziv kategorije je obavezan." });

            var naziv = request.Naziv.Trim();

            // US-30 AC: Kategorija već postoji
            if (await _kategorijaRepository.ExistsByNameAsync(naziv))
                return Conflict(new { message = $"Kategorija \"{naziv}\" već postoji u sistemu." });

            var kategorija = await _kategorijaRepository.CreateAsync(new Kategorija
            {
                Naziv = naziv,
                Opis = string.IsNullOrWhiteSpace(request.Opis) ? null : request.Opis.Trim()
            });

            _cacheVersions.BumpCategoriesVersion();
            _cacheVersions.BumpBooksVersion();

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

            if (SadrziHtml(request.Naziv) || SadrziHtml(request.Opis))
                return BadRequest(new { message = "Naziv/opis ne smije sadržavati HTML tagove." });

            var kategorija = await _kategorijaRepository.GetByIdAsync(id);
            if (kategorija == null)
                return NotFound(new { message = $"Kategorija sa ID {id} nije pronađena." });

            var naziv = request.Naziv.Trim();

            // US-33 AC: Naziv već postoji kod druge kategorije
            if (await _kategorijaRepository.ExistsByNameAsync(naziv, id))
                return Conflict(new { message = $"Kategorija \"{naziv}\" već postoji u sistemu." });

            kategorija.Naziv = naziv;
            kategorija.Opis = string.IsNullOrWhiteSpace(request.Opis) ? null : request.Opis.Trim();
            await _kategorijaRepository.UpdateAsync(kategorija);

            _cacheVersions.BumpCategoriesVersion();
            _cacheVersions.BumpBooksVersion();

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

            _cacheVersions.BumpCategoriesVersion();
            _cacheVersions.BumpBooksVersion();

            return Ok(new
            {
                message = $"Kategorija \"{kategorija.Naziv}\" je uspješno obrisana.",
                id = kategorija.Id
            });
        }

        private static bool SadrziHtml(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            return Regex.IsMatch(input, "<.*?>|&[a-z]+;", RegexOptions.IgnoreCase);
        }
    }

    // DTO za kreiranje/uređivanje kategorije
    public class KategorijaRequest
    {
        public string Naziv { get; set; } = string.Empty;
        public string? Opis { get; set; }
    }

    public class KategorijaResponse
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string? Opis { get; set; }
        public int BrojKnjiga { get; set; }
    }
}