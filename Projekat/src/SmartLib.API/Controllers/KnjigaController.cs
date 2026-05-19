using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public KnjigaController(
            IKnjigaRepository knjigaRepository,
            IPrimjerakRepository primjerakRepository,
            IKategorijaRepository kategorijaRepository,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache)
        {
            _knjigaRepository = knjigaRepository;
            _primjerakRepository = primjerakRepository;
            _kategorijaRepository = kategorijaRepository;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }
        // GET: api/Knjiga/Korice?isbn=...&size=M
        [HttpGet("Korice")]
        [AllowAnonymous]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<IActionResult> Korice([FromQuery] string? isbn, [FromQuery] string size = "M")
        {
            // 1. Primamo nullable string? da izbjegnemo ugrađeni 400 BadRequest,
            // a IsNullOrWhiteSpace hvata i prazan string "" i samo razmake "   "
            if (string.IsNullOrWhiteSpace(isbn)) return NotFound();

            var normalizedIsbn = NormalizeIsbn(isbn);

            // 2. Ako ISBN nakon normalizacije nema 10 ili 13 znakova,
            // odmah znamo da nije validan i vraćamo 404 bez cimanja API-ja.
            if ((normalizedIsbn.Length != 10 && normalizedIsbn.Length != 13) ||
                !System.Text.RegularExpressions.Regex.IsMatch(normalizedIsbn, @"^\d{9}[\dXxf]|\d{13}$"))
            {
                return NotFound();
            }
            var cacheKey = $"cover_{normalizedIsbn}_{size}";

            if (_cache.TryGetValue(cacheKey, out byte[] cachedImage))
            {
                return File(cachedImage, "image/jpeg");
            }

            try
            {
                var upperSize = string.IsNullOrEmpty(size) ? "M" : size.ToUpper();
                if (upperSize != "S" && upperSize != "M" && upperSize != "L")
                {
                    upperSize = "M";
                }

                var client = _httpClientFactory.CreateClient();
                var openLibraryUrl = $"https://covers.openlibrary.org/b/isbn/{normalizedIsbn}-{upperSize}.jpg?default=false";
                var response = await client.GetAsync(openLibraryUrl);

                if (response.IsSuccessStatusCode)
                {
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();

                    // Osiguranje za lažni 200 OK: ako je slika prazna ili premala
                    if (imageBytes != null && imageBytes.Length > 100)
                    {
                        _cache.Set(cacheKey, imageBytes, TimeSpan.FromHours(24));
                        return File(imageBytes, "image/jpeg");
                    }
                }
            }
            catch
            {
                // Fallback: ne radimo ništa, vraćamo 404
            }

            return NotFound();
        }

        // GET: api/Knjiga
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KnjigaDto>>> GetAll(
            [FromQuery] string? naslov,
            [FromQuery] string? autor,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 16)
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
        [HttpGet("{id:int}")]
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

            var kategorija = await _kategorijaRepository.GetByIdAsync(model.KategorijaId);
            if (kategorija == null)
                return BadRequest("Odabrana kategorija nije validna.");

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
            // 1. Provjera aktivnih zaduženja (US-28)
            if (await _knjigaRepository.HasActiveLoansAsync(id))
            {
                return BadRequest(new { poruka = "Knjiga ima aktivna zaduženja i ne može biti obrisana." });
            }

            // 2. Izvršavanje brisanja
            try
            {
                var success = await _knjigaRepository.DeleteAsync(id);

                if (!success)
                    return NotFound(new { poruka = "Knjiga sa tim ID-om nije pronađena." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { poruka = "Sistemska greška prilikom brisanja.", detalji = ex.Message });
            }
        }

        #region Helperi

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

        private static string NormalizeIsbn(string isbn) =>
            isbn.Replace("-", "").Replace(" ", "").Trim();

        private static int MapCoverZoom(string size) => size switch
        {
            "S" => 1,
            "L" => 3,
            _ => 2
        };

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