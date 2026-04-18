using Microsoft.AspNetCore.Mvc;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Inventar modul — Upravljanje fizičkim primjercima knjiga
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PrimjerakController : ControllerBase
    {
        [HttpGet("knjiga/{knjigaId}")]
        public IActionResult GetByKnjiga(int knjigaId)
        {
            // TODO: Pregled svih primjeraka knjige sa statusima
            return Ok(new { message = "TODO" });
        }

        [HttpPost]
        public IActionResult Create(/* TODO: PrimjerakCreateDto */)
        {
            // TODO: Kreiranje novog primjerka za knjugu
            return Ok(new { message = "TODO" });
        }
    }
}
