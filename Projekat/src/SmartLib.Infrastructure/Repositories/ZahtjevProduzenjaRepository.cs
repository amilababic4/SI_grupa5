// Lokacija: SmartLib.Infrastructure/Repositories/ZahtjevProduzenjaRepository.cs
// NOVI FAJL — dodati u projekt i registrirati u DI.

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Services;

namespace SmartLib.Infrastructure.Repositories
{
    public class ZahtjevProduzenjaRepository : IZahtjevProduzenjaRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditLogService _audit;
        private readonly IHttpContextAccessor _httpContext;

        public ZahtjevProduzenjaRepository(
            ApplicationDbContext context,
            AuditLogService audit,
            IHttpContextAccessor httpContext)
        {
            _context = context;
            _audit = audit;
            _httpContext = httpContext;
        }

        // ── Helper: čita KorisnikId iz claims ────────────────────────────
        private int? TrenutniKorisnikId()
        {
            var claim = _httpContext.HttpContext?.User?.FindFirst("korisnikId")
                     ?? _httpContext.HttpContext?.User?.FindFirst("id")
                     ?? _httpContext.HttpContext?.User?.FindFirst(
                            System.Security.Claims.ClaimTypes.NameIdentifier);
            if (claim is null) return null;
            return int.TryParse(claim.Value, out var id) ? id : null;
        }

        // ── Član: dohvatanje aktivnog zahtjeva na čekanju ─────────────────
        public async Task<ZahtjevProduzenja?> GetAktivniZahtjevAsync(int korisnikId)
            => await _context.ZahtjeviProduzenja
                .Include(z => z.Korisnik)
                .Where(z => z.KorisnikId == korisnikId && z.Status == "na_cekanju")
                .OrderByDescending(z => z.DatumPodnosenja)
                .FirstOrDefaultAsync();

        // ── Član: historija svih zahtjeva ────────────────────────────────
        public async Task<IEnumerable<ZahtjevProduzenja>> GetZahtjeviByKorisnikAsync(int korisnikId)
            => await _context.ZahtjeviProduzenja
                .Include(z => z.ObradioKorisnik)
                .Where(z => z.KorisnikId == korisnikId)
                .OrderByDescending(z => z.DatumPodnosenja)
                .ToListAsync();

        // ── Bibliotekar: lista zahtjeva na čekanju ────────────────────────
        public async Task<IEnumerable<ZahtjevProduzenja>> GetNaCekanjuAsync()
            => await _context.ZahtjeviProduzenja
                .Include(z => z.Korisnik)
                .Where(z => z.Status == "na_cekanju")
                .OrderBy(z => z.DatumPodnosenja) // FIFO — najstariji zahtjev prvi
                .ToListAsync();

        // ── Detalji jednog zahtjeva ───────────────────────────────────────
        public async Task<ZahtjevProduzenja?> GetByIdAsync(int id)
            => await _context.ZahtjeviProduzenja
                .Include(z => z.Korisnik)
                .Include(z => z.ObradioKorisnik)
                .FirstOrDefaultAsync(z => z.Id == id);

        // ── Kreiranje novog zahtjeva ──────────────────────────────────────
        public async Task<ZahtjevProduzenja> CreateAsync(ZahtjevProduzenja zahtjev)
        {
            zahtjev.DatumPodnosenja = DateTime.UtcNow;
            zahtjev.Status = "na_cekanju";

            _context.ZahtjeviProduzenja.Add(zahtjev);
            await _context.SaveChangesAsync();

            await _audit.LogCreateAsync(
                new
                {
                    zahtjev.Id,
                    zahtjev.KorisnikId,
                    zahtjev.TrajanjeMjeseci,
                    zahtjev.Status,
                    zahtjev.DatumPodnosenja
                },
                entitetTip: "ZahtjevProduzenja",
                entitetId: zahtjev.Id,
                korisnikId: TrenutniKorisnikId()
            );

            return zahtjev;
        }

        // ── Ažuriranje zahtjeva (odobravanje/odbijanje) ───────────────────
        public async Task UpdateAsync(ZahtjevProduzenja zahtjev)
        {
            var staro = await _context.ZahtjeviProduzenja
                .AsNoTracking()
                .FirstOrDefaultAsync(z => z.Id == zahtjev.Id);

            _context.ZahtjeviProduzenja.Update(zahtjev);
            await _context.SaveChangesAsync();

            if (staro is not null)
            {
                await _audit.LogUpdateAsync(
                    new { staro.Id, staro.Status, staro.KorisnikId },
                    new
                    {
                        zahtjev.Id,
                        zahtjev.Status,
                        zahtjev.KorisnikId,
                        zahtjev.ObradioKorisnikId,
                        zahtjev.DatumObrade,
                        zahtjev.NoviDatumIsteka,
                        zahtjev.RazlogOdbijanja
                    },
                    entitetTip: "ZahtjevProduzenja",
                    entitetId: zahtjev.Id,
                    korisnikId: TrenutniKorisnikId()
                );
            }
        }

        // ── Badge count za bibliotekarsku navigaciju ──────────────────────
        public async Task<int> BrojNaCekanjuAsync()
            => await _context.ZahtjeviProduzenja
                .CountAsync(z => z.Status == "na_cekanju");
    }
}