using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Services;

namespace SmartLib.Infrastructure.Repositories
{
    public class PrimjerakRepository : IPrimjerakRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AuditLogService _audit;
        private readonly IHttpContextAccessor _httpContext;

        public PrimjerakRepository(
            ApplicationDbContext db,
            AuditLogService audit,
            IHttpContextAccessor httpContext)
        {
            _db = db;
            _audit = audit;
            _httpContext = httpContext;
        }

        // ── Helper: čita KorisnikId iz JWT claims ─────────────────────────
        private int? TrenutniKorisnikId()
        {
            var claim = _httpContext.HttpContext?.User?.FindFirst("korisnikId")
                     ?? _httpContext.HttpContext?.User?.FindFirst("id")
                     ?? _httpContext.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (claim is null) return null;
            return int.TryParse(claim.Value, out var id) ? id : null;
        }

        // ── Read metode ───────────────────────────────────────────────────

        public async Task<IEnumerable<Primjerak>> GetByKnjigaAsync(int knjigaId)
        {
            return await _db.Primjerci
                .Where(p => p.KnjigaId == knjigaId)
                .OrderBy(p => p.InventarniBroj)
                .ToListAsync();
        }

        public async Task<Primjerak?> GetByIdAsync(int id)
        {
            return await _db.Primjerci
                .Include(p => p.Knjiga)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> GetAvailableCountAsync(int knjigaId)
        {
            return await _db.Primjerci
                .CountAsync(p => p.KnjigaId == knjigaId && p.Status == "dostupan");
        }

        public async Task<bool> HasActiveZaduzenjeAsync(int primjerakId)
        {
            return await _db.Zaduzenja
                .AnyAsync(z => z.PrimjerakId == primjerakId && z.Status == "aktivno");
        }

        // ── Write metode sa audit logom ───────────────────────────────────

        public async Task<Primjerak> CreateAsync(Primjerak primjerak)
        {
            _db.Primjerci.Add(primjerak);
            await _db.SaveChangesAsync();

            await _audit.LogCreateAsync(
                new { primjerak.Id, primjerak.KnjigaId, primjerak.InventarniBroj, primjerak.Status, primjerak.DatumNabave },
                entitetTip: "Primjerak",
                entitetId: primjerak.Id,
                korisnikId: TrenutniKorisnikId()
            );

            return primjerak;
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var primjerak = await _db.Primjerci.FindAsync(id);
            if (primjerak == null) return;

            var staroStanje = new { primjerak.Id, primjerak.KnjigaId, primjerak.InventarniBroj, primjerak.Status };

            primjerak.Status = status;
            await _db.SaveChangesAsync();

            await _audit.LogUpdateAsync(
                staroStanje,
                new { primjerak.Id, primjerak.KnjigaId, primjerak.InventarniBroj, primjerak.Status },
                entitetTip: "Primjerak",
                entitetId: id,
                korisnikId: TrenutniKorisnikId()
            );
        }

        public async Task DeactivateAsync(int id)
        {
            var primjerak = await _db.Primjerci.FindAsync(id);
            if (primjerak == null) return;

            var staroStanje = new { primjerak.Id, primjerak.KnjigaId, primjerak.InventarniBroj, primjerak.Status };

            primjerak.Status = "deaktiviran";
            await _db.SaveChangesAsync();

            await _audit.LogUpdateAsync(
                staroStanje,
                new { primjerak.Id, primjerak.KnjigaId, primjerak.InventarniBroj, primjerak.Status },
                entitetTip: "Primjerak",
                entitetId: id,
                korisnikId: TrenutniKorisnikId()
            );
        }
    }
}