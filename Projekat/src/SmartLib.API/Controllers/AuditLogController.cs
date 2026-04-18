using Microsoft.AspNetCore.Mvc;

namespace SmartLib.API.Controllers
{
    /// <summary>
    /// Audit/Log modul — Pregled audit log zapisa (samo admin)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            // TODO: Pregled audit log zapisa sa paginacijom (samo admin)
            return Ok(new { message = "TODO" });
        }
    }
}
