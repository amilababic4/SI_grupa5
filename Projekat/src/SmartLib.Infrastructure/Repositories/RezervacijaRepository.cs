using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Services;

namespace SmartLib.Infrastructure.Repositories
{
    public class RezervacijaRepository : IRezervacijaRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AuditLogService _audit;
        private readonly IHttpContextAccessor _httpContext;

        public RezervacijaRepository(
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

        public async Task<IEnumerable<Rezervacija>> GetActiveAsync()
        {
            await CancelExpiredAsync();
            return await _db.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.Knjiga)
                    .ThenInclude(k => k!.Primjerci)
                .Where(r => r.Status == "aktivna")
                .OrderBy(r => r.DatumRezervacije)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rezervacija>> GetByKorisnikAsync(int korisnikId)
        {
            await CancelExpiredAsync();
            return await _db.Rezervacije
                .Include(r => r.Knjiga)
                    .ThenInclude(k => k!.Primjerci)
                .Where(r => r.KorisnikId == korisnikId && r.Status == "aktivna")
                .OrderBy(r => r.DatumRezervacije)
                .ToListAsync();
        }

        public async Task<Rezervacija?> GetByIdAsync(int id)
        {
            return await _db.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.Knjiga)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Rezervacija?> GetNextActiveForBookAsync(int knjigaId)
        {
            await CancelExpiredAsync();
            return await _db.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.Knjiga)
                .Where(r => r.KnjigaId == knjigaId && r.Status == "aktivna")
                .OrderBy(r => r.DatumRezervacije)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasActiveAsync(int korisnikId, int knjigaId)
        {
            return await _db.Rezervacije
                .AnyAsync(r => r.KorisnikId == korisnikId
                            && r.KnjigaId == knjigaId
                            && r.Status == "aktivna");
        }

        // ── Write metode sa audit logom ───────────────────────────────────

        public async Task<Rezervacija> CreateAsync(Rezervacija rezervacija)
        {
            _db.Rezervacije.Add(rezervacija);
            await _db.SaveChangesAsync();

            await _audit.LogCreateAsync(
                new { rezervacija.Id, rezervacija.KorisnikId, rezervacija.KnjigaId, rezervacija.Status, rezervacija.DatumRezervacije, rezervacija.DatumIsteka },
                entitetTip: "Rezervacija",
                entitetId: rezervacija.Id,
                korisnikId: TrenutniKorisnikId()
            );

            return rezervacija;
        }

        public async Task UpdateAsync(Rezervacija rezervacija)
        {
            var staro = await _db.Rezervacije
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == rezervacija.Id);

            _db.Rezervacije.Update(rezervacija);
            await _db.SaveChangesAsync();

            if (staro is not null)
            {
                await _audit.LogUpdateAsync(
                    new { staro.Id, staro.KorisnikId, staro.KnjigaId, staro.Status, staro.DatumRezervacije, staro.DatumIsteka },
                    new { rezervacija.Id, rezervacija.KorisnikId, rezervacija.KnjigaId, rezervacija.Status, rezervacija.DatumRezervacije, rezervacija.DatumIsteka },
                    entitetTip: "Rezervacija",
                    entitetId: rezervacija.Id,
                    korisnikId: TrenutniKorisnikId()
                );
            }
        }

        public async Task FulfillAsync(int korisnikId, int knjigaId)
        {
            var rezervacija = await _db.Rezervacije
                .FirstOrDefaultAsync(r => r.KorisnikId == korisnikId
                                       && r.KnjigaId == knjigaId
                                       && r.Status == "aktivna");
            if (rezervacija == null) return;

            var staroStanje = new { rezervacija.Id, rezervacija.KorisnikId, rezervacija.KnjigaId, rezervacija.Status };

            rezervacija.Status = "realizovana";
            await _db.SaveChangesAsync();

            await _audit.LogUpdateAsync(
                staroStanje,
                new { rezervacija.Id, rezervacija.KorisnikId, rezervacija.KnjigaId, rezervacija.Status },
                entitetTip: "Rezervacija",
                entitetId: rezervacija.Id,
                korisnikId: TrenutniKorisnikId()
            );
        }

        public async Task CancelExpiredAsync()
        {
            var istekle = await _db.Rezervacije
                .Where(r => r.Status == "aktivna" && r.DatumIsteka < DateTime.UtcNow)
                .ToListAsync();

            if (istekle.Count == 0) return;

            foreach (var r in istekle)
                r.Status = "istekla";

            await _db.SaveChangesAsync();

            // Logujemo svaku isteklu rezervaciju pojedinačno
            foreach (var r in istekle)
            {
                await _audit.LogUpdateAsync(
                    new { r.Id, r.KorisnikId, r.KnjigaId, Status = "aktivna", r.DatumIsteka },
                    new { r.Id, r.KorisnikId, r.KnjigaId, r.Status, r.DatumIsteka },
                    entitetTip: "Rezervacija",
                    entitetId: r.Id,
                    korisnikId: null  // sistem automatski gasi istekle, nema ulogovanog korisnika
                );
            }
        }
    }
}