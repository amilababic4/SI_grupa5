using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Inventar modul — Upravljanje fizičkim primjercima knjiga
    /// US-21: Dodavanje primjeraka | US-22: Pregled | US-23: Statusi | US-24: Deaktivacija
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Bibliotekar,Administrator")]
    public class PrimjerakController : ControllerBase
    {
        private readonly IPrimjerakRepository _primjerakRepository;
        private readonly IKnjigaRepository _knjigaRepository;

        public PrimjerakController(
            IPrimjerakRepository primjerakRepository,
            IKnjigaRepository knjigaRepository)
        {
            _primjerakRepository = primjerakRepository;
            _knjigaRepository = knjigaRepository;
        }

        // US-22 + US-23: Pregled svih primjeraka knjige sa statusima
        [HttpGet("knjiga/{knjigaId}")]
        public async Task<IActionResult> GetByKnjiga(int knjigaId)
        {
            var knjiga = await _knjigaRepository.GetByIdAsync(knjigaId);
            if (knjiga == null)
                return NotFound(new { message = $"Knjiga sa ID {knjigaId} nije pronađena." });

            var primjerci = await _primjerakRepository.GetByKnjigaAsync(knjigaId);

            var result = primjerci.Select(p => new
            {
                p.Id,
                p.InventarniBroj,
                p.Status,
                DatumNabave = p.DatumNabave?.ToString("yyyy-MM-dd"),
                p.KnjigaId
            });

            return Ok(result);
        }

        // US-22 + US-23: Pregled pojedinačnog primjerka
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _primjerakRepository.GetByIdAsync(id);
            if (p == null)
                return NotFound(new { message = $"Primjerak sa ID {id} nije pronađen." });

            return Ok(new
            {
                p.Id,
                p.InventarniBroj,
                p.Status,
                DatumNabave = p.DatumNabave?.ToString("yyyy-MM-dd"),
                p.KnjigaId,
                Knjiga = p.Knjiga?.Naslov
            });
        }

        // US-21: Kreiranje novih primjeraka za postojeću knjigu
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PrimjerakCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var knjiga = await _knjigaRepository.GetByIdAsync(request.KnjigaId);
            if (knjiga == null)
                return BadRequest(new { message = "Primjerak mora biti povezan s postojećom knjigom." });

            if (request.BrojNovih < 1 || request.BrojNovih > 50)
                return BadRequest(new { message = "Broj primjeraka mora biti između 1 i 50." });

            var postojeci = (await _primjerakRepository.GetByKnjigaAsync(request.KnjigaId)).ToList();
            int sljedeciRedni = postojeci.Count + 1;

            var kreirani = new List<object>();

            for (int i = 0; i < request.BrojNovih; i++)
            {
                var primjerak = await _primjerakRepository.CreateAsync(new Primjerak
                {
                    KnjigaId = request.KnjigaId,
                    InventarniBroj = $"INV-{request.KnjigaId}-{sljedeciRedni + i:D3}",
                    Status = "dostupan",
                    DatumNabave = DateTime.UtcNow
                });

                kreirani.Add(new
                {
                    primjerak.Id,
                    primjerak.InventarniBroj,
                    primjerak.Status,
                    DatumNabave = primjerak.DatumNabave?.ToString("yyyy-MM-dd")
                });
            }

            return CreatedAtAction(
                nameof(GetByKnjiga),
                new { knjigaId = request.KnjigaId },
                new
                {
                    message = $"Uspješno kreirano {request.BrojNovih} primjerak(a) za knjigu '{knjiga.Naslov}'.",
                    kreirani
                });
        }

        // US-24: Deaktivacija primjerka
        [HttpPost("{id}/deaktiviraj")]
        public async Task<IActionResult> Deaktiviraj(int id)
        {
            var primjerak = await _primjerakRepository.GetByIdAsync(id);
            if (primjerak == null)
                return NotFound(new { message = $"Primjerak sa ID {id} nije pronađen." });

            if (primjerak.Status == "deaktiviran")
                return Conflict(new { message = $"Primjerak {primjerak.InventarniBroj} je već deaktiviran." });

            // AC: Ne smije se deaktivirati primjerak koji je aktivno zadužen
            if (await _primjerakRepository.HasActiveZaduzenjeAsync(id))
                return Conflict(new
                {
                    message = $"Primjerak {primjerak.InventarniBroj} je trenutno aktivo zadužen i ne može biti deaktiviran."
                });

            await _primjerakRepository.DeactivateAsync(id);

            return Ok(new
            {
                message = $"Primjerak {primjerak.InventarniBroj} je uspješno deaktiviran.",
                id = primjerak.Id,
                inventarniBroj = primjerak.InventarniBroj,
                noviStatus = "deaktiviran"
            });
        }
    }

    // DTO za kreiranje — inline jer je jednostavan i specifičan za ovaj controller
    public class PrimjerakCreateRequest
    {
        public int KnjigaId { get; set; }
        public int BrojNovih { get; set; } = 1;
    }
}