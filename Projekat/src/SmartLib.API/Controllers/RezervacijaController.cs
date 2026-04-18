using Microsoft.AspNetCore.Mvc;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Rezervacije modul — Evidencija rezervacija nedostupnih knjiga
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RezervacijaController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetActive()
        {
            // TODO: Pregled aktivnih rezervacija (bibliotekar)
            return Ok(new { message = "TODO" });
        }

        [HttpGet("moje")]
        public IActionResult GetMine()
        {
            // TODO: Pregled vlastitih rezervacija (član)
            return Ok(new { message = "TODO" });
        }

        [HttpPost]
        public IActionResult Create(/* TODO: RezervacijaCreateDto */)
        {
            // TODO: Kreiranje rezervacije za nedostupnu knjigu
            return Ok(new { message = "TODO" });
        }

        [HttpDelete("{id}")]
        public IActionResult Cancel(int id)
        {
            // TODO: Otkazivanje rezervacije (član)
            return Ok(new { message = "TODO" });
        }
    }
}
