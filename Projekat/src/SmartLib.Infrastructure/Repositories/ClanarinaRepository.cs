using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Services;

namespace SmartLib.Infrastructure.Repositories
{
    public class ClanarinaRepository : IClanarinaRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditLogService _audit;
        private readonly IHttpContextAccessor _httpContext;

        public ClanarinaRepository(
            ApplicationDbContext context,
            AuditLogService audit,
            IHttpContextAccessor httpContext)
        {
            _context = context;
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

        public async Task<Clanarina?> GetByKorisnikAsync(int korisnikId)
            => await _context.Clanarine
                .Where(c => c.KorisnikId == korisnikId)
                .OrderByDescending(c => c.DatumIsteka)
                .FirstOrDefaultAsync();

        public async Task<bool> IsActiveAsync(int korisnikId)
            => await _context.Clanarine
                .AnyAsync(c => c.KorisnikId == korisnikId
                            && c.DatumIsteka.Date >= DateTime.UtcNow.Date);

        // ── Write metode sa audit logom ───────────────────────────────────

        public async Task<Clanarina> CreateAsync(Clanarina clanarina)
        {
            clanarina.Status = DateTime.UtcNow.Date <= clanarina.DatumIsteka.Date
                ? "aktivna" : "istekla";

            _context.Clanarine.Add(clanarina);
            await _context.SaveChangesAsync();

            await _audit.LogCreateAsync(
                new { clanarina.Id, clanarina.KorisnikId, clanarina.Status, clanarina.DatumIsteka },
                entitetTip: "Clanarina",
                entitetId: clanarina.Id,
                korisnikId: TrenutniKorisnikId()
            );

            return clanarina;
        }

        public async Task UpdateAsync(Clanarina clanarina)
        {
            var staro = await _context.Clanarine
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == clanarina.Id);

            clanarina.Status = DateTime.UtcNow.Date <= clanarina.DatumIsteka.Date
                ? "aktivna" : "istekla";

            _context.Clanarine.Update(clanarina);
            await _context.SaveChangesAsync();

            if (staro is not null)
            {
                await _audit.LogUpdateAsync(
                    new { staro.Id, staro.KorisnikId, staro.Status, staro.DatumIsteka },
                    new { clanarina.Id, clanarina.KorisnikId, clanarina.Status, clanarina.DatumIsteka },
                    entitetTip: "Clanarina",
                    entitetId: clanarina.Id,
                    korisnikId: TrenutniKorisnikId()
                );
            }
        }
    }
}