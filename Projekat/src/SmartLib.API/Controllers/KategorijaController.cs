using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Katalog modul — Upravljanje kategorijama knjiga
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Bibliotekar,Administrator")]
    public class KategorijaController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            // TODO: Pregled svih kategorija
            return Ok(new { message = "TODO" });
        }

        [HttpPost]
        public IActionResult Create(/* TODO: KategorijaDto */)
        {
            // TODO: Dodavanje kategorije
            return Ok(new { message = "TODO" });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id)
        {
            // TODO: Uređivanje kategorije
            return Ok(new { message = "TODO" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // TODO: Brisanje kategorije (zabrana ako ima povezane knjige)
            return Ok(new { message = "TODO" });
        }
    }
}
