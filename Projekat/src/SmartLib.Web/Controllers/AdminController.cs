using Microsoft.AspNetCore.Mvc;
using SmartLib.Core.Interfaces;

namespace SmartLib.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAuditLogRepository _auditRepo;
        private readonly IKorisnikRepository _korisnikRepo;

        public AdminController(IAuditLogRepository auditRepo, IKorisnikRepository korisnikRepo)
        {
            _auditRepo = auditRepo;
            _korisnikRepo = korisnikRepo;
        }

        // ── Korisnici ─────────────────────────────────────────────────────────

        public async Task<IActionResult> Korisnici()
        {
            var korisnici = await _korisnikRepo.GetAllAsync();
            return View(korisnici);
        }

        // ── Audit Log ─────────────────────────────────────────────────────────

        public async Task<IActionResult> AuditLog(
            int page = 1,
            int pageSize = 30,
            string? entitetTip = null,
            string? akcija = null,
            int? korisnikId = null,
            DateTime? odDatuma = null,
            DateTime? doDatuma = null)
        {
            var logovi = await _auditRepo.GetFilteredAsync(
                page, pageSize, entitetTip, akcija, korisnikId, odDatuma, doDatuma);

            var ukupno = await _auditRepo.GetTotalCountAsync(entitetTip, akcija, korisnikId);

            ViewBag.Stranica = page;
            ViewBag.PageSize = pageSize;
            ViewBag.UkupnoStrana = (int)Math.Ceiling(ukupno / (double)pageSize);
            ViewBag.UkupnoBroj = ukupno;
            ViewBag.EntitetTip = entitetTip;
            ViewBag.Akcija = akcija;
            ViewBag.KorisnikId = korisnikId;
            ViewBag.OdDatuma = odDatuma?.ToString("yyyy-MM-dd");
            ViewBag.DoDatuma = doDatuma?.ToString("yyyy-MM-dd");

            return View(logovi);
        }
    }
}