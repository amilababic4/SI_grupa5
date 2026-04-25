using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Članarina modul — Upravljanje članstvom korisnika
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Bibliotekar,Administrator")]
    public class ClanarinaController : ControllerBase
    {
        [HttpGet("{korisnikId}")]
        public IActionResult GetByKorisnik(int korisnikId)
        {
            // TODO: Pregled članarine korisnika
            return Ok(new { message = "TODO" });
        }

        [HttpPost]
        public IActionResult Create(/* TODO: ClanarinaCreateDto */)
        {
            // TODO: Evidentiranje nove članarine
            return Ok(new { message = "TODO" });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id /*, TODO: ClanarinaUpdateDto */)
        {
            // TODO: Ažuriranje/produženje članarine
            return Ok(new { message = "TODO" });
        }
    }
}
