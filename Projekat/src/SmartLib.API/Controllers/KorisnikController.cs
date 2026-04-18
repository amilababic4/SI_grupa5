using Microsoft.AspNetCore.Mvc;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Korisnici modul — Upravljanje korisničkim nalozima
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class KorisnikController : ControllerBase
    {
        // TODO: Inject IKorisnikRepository

        [HttpGet]
        public IActionResult GetAll()
        {
            // TODO: Pregled svih korisnika (admin) ili članova (bibliotekar)
            return Ok(new { message = "TODO" });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // TODO: Pregled profila korisnika
            return Ok(new { message = "TODO" });
        }

        [HttpPost]
        public IActionResult Create(/* TODO: KorisnikCreateDto */)
        {
            // TODO: Kreiranje novog člana (bibliotekar/admin)
            return Ok(new { message = "TODO" });
        }

        [HttpPut("{id}/uloga")]
        public IActionResult ChangeRole(int id /*, TODO: ChangeRoleDto */)
        {
            // TODO: Promjena uloge (samo admin)
            return Ok(new { message = "TODO" });
        }

        [HttpPut("{id}/deaktiviraj")]
        public IActionResult Deactivate(int id)
        {
            // TODO: Deaktivacija naloga (soft delete)
            return Ok(new { message = "TODO" });
        }
    }
}
