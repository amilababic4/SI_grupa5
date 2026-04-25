using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Zaduženja modul — Evidencija zaduživanja i vraćanja knjiga
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ZaduzenjeController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetActive()
        {
            // TODO: Pregled aktivnih zaduženja (bibliotekar)
            return Ok(new { message = "TODO" });
        }

        [HttpGet("moja")]
        public IActionResult GetMine()
        {
            // TODO: Pregled vlastitih zaduženja (član)
            return Ok(new { message = "TODO" });
        }

        [HttpGet("historija/{korisnikId}")]
        public IActionResult GetHistory(int korisnikId)
        {
            // TODO: Historija zaduženja člana
            return Ok(new { message = "TODO" });
        }

        [HttpPost("zaduzi")]
        public IActionResult Zaduzi(/* TODO: ZaduzenjeCreateDto */)
        {
            // TODO: Kreiranje zaduženja (atomska transakcija)
            // 1. Provjera aktivnosti članarine
            // 2. Provjera dostupnosti primjerka
            // 3. Kreiranje zaduženja + promjena statusa primjerka + audit log
            return Ok(new { message = "TODO" });
        }

        [HttpPost("vrati/{id}")]
        public IActionResult Vrati(int id)
        {
            // TODO: Vraćanje knjige (atomska transakcija)
            // 1. Zatvaranje zaduženja + promjena statusa primjerka + audit log
            return Ok(new { message = "TODO" });
        }
    }
}
