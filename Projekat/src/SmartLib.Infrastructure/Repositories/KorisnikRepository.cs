using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Services;

namespace SmartLib.Infrastructure.Repositories
{
    public class KorisnikRepository : IKorisnikRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AuditLogService _audit;
        private readonly IHttpContextAccessor _httpContext;

        public KorisnikRepository(
            ApplicationDbContext db,
            AuditLogService audit,
            IHttpContextAccessor httpContext)
        {
            _db = db;
            _audit = audit;
            _httpContext = httpContext;
        }

        // ── Helper: čita KorisnikId iz JWT claims ─────────────────────────
        // Prilagodi claim name ako koristiš drugačiji (npr. "id", "sub", "korisnikId")
        private int? TrenutniKorisnikId()
        {
            var claim = _httpContext.HttpContext?.User?.FindFirst("korisnikId")
                     ?? _httpContext.HttpContext?.User?.FindFirst("id")
                     ?? _httpContext.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (claim is null) return null;
            return int.TryParse(claim.Value, out var id) ? id : null;
        }

        // ── Read metode (nepromijenjene) ──────────────────────────────────

        public async Task<IEnumerable<Korisnik>> GetAllAsync()
        {
            return await _db.Korisnici
                .Include(k => k.Uloga)
                .ToListAsync();
        }

        public async Task<Korisnik?> GetByIdAsync(int id)
        {
            return await _db.Korisnici
                .Include(k => k.Uloga)
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<Korisnik?> GetByEmailAsync(string email)
        {
            var normalized = email.Trim().ToLowerInvariant();
            return await _db.Korisnici
                .Include(k => k.Uloga)
                .FirstOrDefaultAsync(k => k.Email == normalized);
        }

        public async Task<Korisnik?> GetByResetTokenAsync(string token)
        {
            return await _db.Korisnici
                .Include(k => k.Uloga)
                .FirstOrDefaultAsync(k => k.ResetToken == token && k.Status == "aktivan");
        }

        // ── Write metode sa audit logom ───────────────────────────────────

        public async Task<Korisnik> CreateAsync(Korisnik korisnik)
        {
            _db.Korisnici.Add(korisnik);
            await _db.SaveChangesAsync();

            await _audit.LogCreateAsync(
                new { korisnik.Id, korisnik.Ime, korisnik.Prezime, korisnik.Email, korisnik.UlogaId, korisnik.Status, korisnik.DatumKreiranja },
                entitetTip: "Korisnik",
                entitetId: korisnik.Id,
                korisnikId: TrenutniKorisnikId()
            );

            return korisnik;
        }

        public async Task UpdateAsync(Korisnik korisnik)
        {
            var staro = await _db.Korisnici
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.Id == korisnik.Id);

            _db.Korisnici.Update(korisnik);
            await _db.SaveChangesAsync();

            if (staro is not null)
            {
                await _audit.LogUpdateAsync(
                    new { staro.Id, staro.Ime, staro.Prezime, staro.Email, staro.UlogaId, staro.Status },
                    new { korisnik.Id, korisnik.Ime, korisnik.Prezime, korisnik.Email, korisnik.UlogaId, korisnik.Status },
                    entitetTip: "Korisnik",
                    entitetId: korisnik.Id,
                    korisnikId: TrenutniKorisnikId()
                );
            }
        }

        public async Task<int> DeleteDeactivatedOlderThanAsync(DateTime cutoffUtc)
        {
            return await _db.Database.ExecuteSqlInterpolatedAsync($@"
                DELETE FROM Korisnici
                WHERE Status = {"deaktiviran"}
                  AND DatumDeaktivacije IS NOT NULL
                  AND DatumDeaktivacije <= {cutoffUtc}
                  AND NOT EXISTS (SELECT 1 FROM Zaduzenja z WHERE z.KorisnikId = Korisnici.Id)
                  AND NOT EXISTS (SELECT 1 FROM Rezervacije r WHERE r.KorisnikId = Korisnici.Id)
                  AND NOT EXISTS (SELECT 1 FROM Clanarine c WHERE c.KorisnikId = Korisnici.Id)
            ");
        }
    }
}