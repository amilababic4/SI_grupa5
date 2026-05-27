using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Services;

namespace SmartLib.Infrastructure.Repositories
{
    public class ZaduzenjeRepository : IZaduzenjeRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AuditLogService _audit;
        private readonly IHttpContextAccessor _httpContext;

        public ZaduzenjeRepository(
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

        // ── Read metode (nepromijenjene) ──────────────────────────────────

        public async Task<IEnumerable<Zaduzenje>> GetActiveAsync()
        {
            return await _db.Zaduzenja
                .Include(z => z.Korisnik)
                .Include(z => z.Primjerak)
                    .ThenInclude(p => p!.Knjiga)
                .Where(z => z.Status == "aktivno")
                .OrderBy(z => z.DatumPlaniranogVracanja)
                .ToListAsync();
        }

        public async Task<IEnumerable<Zaduzenje>> GetByKorisnikAsync(int korisnikId)
        {
            return await _db.Zaduzenja
                .Include(z => z.Korisnik)
                .Include(z => z.Primjerak)
                    .ThenInclude(p => p!.Knjiga)
                .Where(z => z.KorisnikId == korisnikId && z.Status == "aktivno")
                .OrderBy(z => z.DatumPlaniranogVracanja)
                .ToListAsync();
        }

        public async Task<IEnumerable<Zaduzenje>> GetHistoryByKorisnikAsync(int korisnikId)
        {
            return await _db.Zaduzenja
                .Include(z => z.Korisnik)
                .Include(z => z.Primjerak)
                    .ThenInclude(p => p!.Knjiga)
                .Where(z => z.KorisnikId == korisnikId && z.Status != "aktivno")
                .OrderByDescending(z => z.DatumStvarnogVracanja)
                .ToListAsync();
        }

        public async Task<Zaduzenje?> GetByIdAsync(int id)
        {
            return await _db.Zaduzenja
                .Include(z => z.Korisnik)
                .Include(z => z.Primjerak)
                    .ThenInclude(p => p!.Knjiga)
                .FirstOrDefaultAsync(z => z.Id == id);
        }

        public async Task<IEnumerable<Zaduzenje>> GetClosedSinceAsync(DateTime granica)
        {
            return await _db.Zaduzenja
                .Include(z => z.Korisnik)
                    .ThenInclude(k => k!.Uloga)
                .Include(z => z.Primjerak)
                    .ThenInclude(p => p!.Knjiga)
                .Where(z => z.Status == "zatvoreno" && z.DatumStvarnogVracanja >= granica)
                .OrderByDescending(z => z.DatumStvarnogVracanja)
                .ToListAsync();
        }

        public async Task<IEnumerable<Zaduzenje>> GetClosedHistoryForKorisnikAsync(int korisnikId, DateTime granica)
        {
            return await _db.Zaduzenja
                .AsNoTracking()
                .Include(z => z.Korisnik)
                .Include(z => z.Primjerak)
                    .ThenInclude(p => p!.Knjiga)
                .Where(z => z.KorisnikId == korisnikId &&
                            z.Status == "zatvoreno" &&
                            z.DatumStvarnogVracanja.HasValue &&
                            z.DatumStvarnogVracanja.Value >= granica)
                .OrderByDescending(z => z.DatumStvarnogVracanja)
                .ToListAsync();
        }

        public async Task<IEnumerable<Zaduzenje>> GetByPrimjerakAsync(int primjerakId)
        {
            return await _db.Zaduzenja
                .Include(z => z.Korisnik)
                    .ThenInclude(k => k!.Uloga)
                .Include(z => z.Primjerak)
                    .ThenInclude(p => p!.Knjiga)
                .Where(z => z.PrimjerakId == primjerakId)
                .OrderByDescending(z => z.DatumZaduzivanja)
                .ToListAsync();
        }

        // ── Write metode sa audit logom ───────────────────────────────────

        public async Task<Zaduzenje> CreateAsync(Zaduzenje zaduzenje)
        {
            _db.Zaduzenja.Add(zaduzenje);
            await _db.SaveChangesAsync();

            await _audit.LogCreateAsync(
                new { zaduzenje.Id, zaduzenje.KorisnikId, zaduzenje.PrimjerakId, zaduzenje.Status, zaduzenje.DatumZaduzivanja, zaduzenje.DatumPlaniranogVracanja },
                entitetTip: "Zaduzenje",
                entitetId: zaduzenje.Id,
                korisnikId: TrenutniKorisnikId()
            );

            return zaduzenje;
        }

        public async Task UpdateAsync(Zaduzenje zaduzenje)
        {
            var staro = await _db.Zaduzenja
                .AsNoTracking()
                .FirstOrDefaultAsync(z => z.Id == zaduzenje.Id);

            _db.Zaduzenja.Update(zaduzenje);
            await _db.SaveChangesAsync();

            if (staro is not null)
            {
                await _audit.LogUpdateAsync(
                    new { staro.Id, staro.KorisnikId, staro.PrimjerakId, staro.Status, staro.DatumZaduzivanja, staro.DatumPlaniranogVracanja, staro.DatumStvarnogVracanja },
                    new { zaduzenje.Id, zaduzenje.KorisnikId, zaduzenje.PrimjerakId, zaduzenje.Status, zaduzenje.DatumZaduzivanja, zaduzenje.DatumPlaniranogVracanja, zaduzenje.DatumStvarnogVracanja },
                    entitetTip: "Zaduzenje",
                    entitetId: zaduzenje.Id,
                    korisnikId: TrenutniKorisnikId()
                );
            }
        }
    }
}