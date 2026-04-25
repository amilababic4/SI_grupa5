using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Katalog modul — Upravljanje knjigama (CRUD, pretraga, filtriranje)
    /// </summary>
    [ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Bibliotekar,Administrator")]
public class KnjigaController : ControllerBase
{
        // TODO: Inject IKnjigaRepository

        [HttpGet]
        public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Pregled kataloga sa paginacijom
            return Ok(new { message = "TODO" });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // TODO: Detalji knjige sa statusom dostupnosti
            return Ok(new { message = "TODO" });
        }

        [HttpGet("pretraga")]
        public IActionResult Search([FromQuery] string? naslov, [FromQuery] string? autor)
        {
            // TODO: Pretraga po naslovu i autoru (case-insensitive)
            return Ok(new { message = "TODO" });
        }

        [HttpPost]
        public IActionResult Create(/* TODO: KnjigaCreateDto */)
        {
            // TODO: Dodavanje nove knjige (bibliotekar/admin)
            return Ok(new { message = "TODO" });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id /*, TODO: KnjigaUpdateDto */)
        {
            // TODO: Uređivanje knjige
            return Ok(new { message = "TODO" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // TODO: Brisanje knjige (provjera aktivnih zaduženja)
            return Ok(new { message = "TODO" });
        }
    }
}
