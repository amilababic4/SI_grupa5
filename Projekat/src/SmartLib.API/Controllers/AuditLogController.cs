using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleNames.Administrator)]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogRepository _auditRepo;

        public AuditLogController(IAuditLogRepository auditRepo)
        {
            _auditRepo = auditRepo;
        }

        /// <summary>
        /// GET /api/auditlog?page=1&pageSize=50&entitetTip=Korisnik&akcija=UPDATE
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? entitetTip = null,
            [FromQuery] string? akcija = null,
            [FromQuery] int? korisnikId = null,
            [FromQuery] DateTime? odDatuma = null,
            [FromQuery] DateTime? doDatuma = null)
        {
            var logovi = await _auditRepo.GetFilteredAsync(page, pageSize, entitetTip, akcija, korisnikId, odDatuma, doDatuma);
            var ukupno = await _auditRepo.GetTotalCountAsync(entitetTip, akcija, korisnikId);

            return Ok(new
            {
                Stranica = page,
                VelicinaStrane = pageSize,
                UkupnoBroj = ukupno,
                Podaci = logovi
            });
        }

        /// <summary>
        /// GET /api/auditlog/entitet/Knjiga/5
        /// Vraća kompletnu historiju promjena za jedan konkretan entitet.
        /// </summary>
        [HttpGet("entitet/{entitetTip}/{entitetId:int}")]
        public async Task<IActionResult> GetByEntitet(string entitetTip, int entitetId)
        {
            var logovi = await _auditRepo.GetByEntitetAsync(entitetTip, entitetId);
            return Ok(logovi);
        }
    }
}