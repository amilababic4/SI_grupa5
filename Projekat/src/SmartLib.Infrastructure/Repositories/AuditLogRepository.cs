using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _db;

        public AuditLogRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        // ── Postojeće ─────────────────────────────────────────────────────────

        public async Task<IEnumerable<AuditLog>> GetAllAsync(int page, int pageSize)
        {
            return await _db.AuditLogs
                .Include(a => a.Korisnik)
                .AsNoTracking()
                .OrderByDescending(a => a.DatumAkcije)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task CreateAsync(AuditLog auditLog)
        {
            _db.AuditLogs.Add(auditLog);
            await _db.SaveChangesAsync();
        }

        // ── Novo ──────────────────────────────────────────────────────────────

        public async Task<int> GetTotalCountAsync(
            string? entitetTip = null,
            string? akcija = null,
            int? korisnikId = null)
        {
            var query = BuildQuery(entitetTip, akcija, korisnikId, null, null);
            return await query.CountAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetFilteredAsync(
            int page,
            int pageSize,
            string? entitetTip = null,
            string? akcija = null,
            int? korisnikId = null,
            DateTime? odDatuma = null,
            DateTime? doDatuma = null)
        {
            var query = BuildQuery(entitetTip, akcija, korisnikId, odDatuma, doDatuma);

            return await query
                .Include(a => a.Korisnik)
                .AsNoTracking()
                .OrderByDescending(a => a.DatumAkcije)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByEntitetAsync(string entitetTip, int entitetId)
        {
            return await _db.AuditLogs
                .Include(a => a.Korisnik)
                .AsNoTracking()
                .Where(a => a.EntitetTip == entitetTip && a.EntitetId == entitetId)
                .OrderByDescending(a => a.DatumAkcije)
                .ToListAsync();
        }

        // ── Privatni helper ───────────────────────────────────────────────────

        private IQueryable<AuditLog> BuildQuery(
            string? entitetTip,
            string? akcija,
            int? korisnikId,
            DateTime? odDatuma,
            DateTime? doDatuma)
        {
            var query = _db.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(entitetTip))
                query = query.Where(a => a.EntitetTip == entitetTip);

            if (!string.IsNullOrWhiteSpace(akcija))
                query = query.Where(a => a.Akcija == akcija);

            if (korisnikId.HasValue)
                query = query.Where(a => a.KorisnikId == korisnikId.Value);

            if (odDatuma.HasValue)
                query = query.Where(a => a.DatumAkcije >= odDatuma.Value);

            if (doDatuma.HasValue)
                query = query.Where(a => a.DatumAkcije <= doDatuma.Value.AddDays(1));

            return query;
        }
    }
}