using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using SmartLib.Infrastructure.Services;

namespace SmartLib.Infrastructure.Repositories
{
    public class KnjigaRepository : IKnjigaRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AuditLogService _audit;
        private readonly IHttpContextAccessor _httpContext;

        public KnjigaRepository(
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

        public async Task<IEnumerable<Knjiga>> GetAllAsync(int page, int pageSize)
        {
            return await _db.Knjige
                .Include(k => k.Kategorija)
                .Include(k => k.Primjerci)
                .AsNoTracking()
                .OrderBy(k => k.Naslov)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Knjiga?> GetByIdAsync(int id)
        {
            return await _db.Knjige
                .Include(k => k.Kategorija)
                .Include(k => k.Primjerci)
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<IEnumerable<Knjiga>> SearchAsync(string? naslov, string? autor)
        {
            var query = _db.Knjige
                .Include(k => k.Kategorija)
                .Include(k => k.Primjerci)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(naslov))
                query = query.Where(k => k.Naslov.ToLower().Contains(naslov.ToLower()));

            if (!string.IsNullOrWhiteSpace(autor))
                query = query.Where(k => k.Autor.ToLower().Contains(autor.ToLower()));

            return await query.OrderBy(k => k.Naslov).ToListAsync();
        }

        public async Task<(IEnumerable<Knjiga> Knjige, int UkupnoBroj)> GetPagedAsync(
            string? naslov,
            string? autor,
            int page,
            int pageSize,
            int? kategorijaId = null,
            string? izdavac = null,
            int? godinaIzdanja = null)
        {
            var query = _db.Knjige
                .Include(k => k.Kategorija)
                .Include(k => k.Primjerci)
                .Include(k => k.Recenzije)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(naslov))
                query = query.Where(k => k.Naslov.ToLower().Contains(naslov.ToLower()));

            if (!string.IsNullOrWhiteSpace(autor))
                query = query.Where(k => k.Autor.ToLower().Contains(autor.ToLower()));

            if (kategorijaId.HasValue)
                query = query.Where(k => k.KategorijaId == kategorijaId.Value);

            if (!string.IsNullOrWhiteSpace(izdavac))
                query = query.Where(k => k.Izdavac != null &&
                                         k.Izdavac.ToLower().Contains(izdavac.ToLower()));

            if (godinaIzdanja.HasValue)
                query = query.Where(k => k.GodinaIzdanja == godinaIzdanja.Value);

            var ukupno = await query.CountAsync();
            var knjige = await query
                .OrderBy(k => k.Naslov)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (knjige, ukupno);
        }

        public async Task<IEnumerable<string>> GetDistinctIzdavaciAsync()
        {
            return await _db.Knjige
                .AsNoTracking()
                .Where(k => k.Izdavac != null && k.Izdavac != "")
                .Select(k => k.Izdavac!)
                .Distinct()
                .OrderBy(i => i)
                .ToListAsync();
        }

        public async Task<IEnumerable<int>> GetDistinctGodineAsync()
        {
            return await _db.Knjige
                .AsNoTracking()
                .Where(k => k.GodinaIzdanja.HasValue)
                .Select(k => k.GodinaIzdanja!.Value)
                .Distinct()
                .OrderByDescending(g => g)
                .ToListAsync();
        }

        public async Task<Knjiga?> GetByIsbnAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn)) return null;
            return await _db.Knjige
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.Isbn == isbn);
        }

        // ── Write metode sa audit logom ───────────────────────────────────

        public async Task<Knjiga> CreateAsync(Knjiga knjiga)
        {
            _db.Knjige.Add(knjiga);
            await _db.SaveChangesAsync();

            await _audit.LogCreateAsync(
                new { knjiga.Id, knjiga.Naslov, knjiga.Autor, knjiga.Isbn, knjiga.KategorijaId, knjiga.Izdavac, knjiga.GodinaIzdanja },
                entitetTip: "Knjiga",
                entitetId: knjiga.Id,
                korisnikId: TrenutniKorisnikId()
            );

            return knjiga;
        }

        public async Task UpdateAsync(Knjiga knjiga)
        {
            var staro = await _db.Knjige
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.Id == knjiga.Id);

            _db.Knjige.Update(knjiga);
            await _db.SaveChangesAsync();

            if (staro is not null)
            {
                await _audit.LogUpdateAsync(
                    new { staro.Id, staro.Naslov, staro.Autor, staro.Isbn, staro.KategorijaId, staro.Izdavac, staro.GodinaIzdanja, staro.Opis },
                    new { knjiga.Id, knjiga.Naslov, knjiga.Autor, knjiga.Isbn, knjiga.KategorijaId, knjiga.Izdavac, knjiga.GodinaIzdanja, knjiga.Opis },
                    entitetTip: "Knjiga",
                    entitetId: knjiga.Id,
                    korisnikId: TrenutniKorisnikId()
                );
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var knjiga = await _db.Knjige
                .Include(k => k.Primjerci)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (knjiga == null) return false;

            // Audit PRIJE brisanja
            await _audit.LogDeleteAsync(
                new { knjiga.Id, knjiga.Naslov, knjiga.Autor, knjiga.Isbn, knjiga.KategorijaId, knjiga.Izdavac, knjiga.GodinaIzdanja },
                entitetTip: "Knjiga",
                entitetId: knjiga.Id,
                korisnikId: TrenutniKorisnikId()
            );

            if (knjiga.Primjerci != null && knjiga.Primjerci.Any())
                _db.Primjerci.RemoveRange(knjiga.Primjerci);

            _db.Knjige.Remove(knjiga);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasActiveLoansAsync(int id)
        {
            return await _db.Zaduzenja
                .AnyAsync(z => z.Primjerak!.KnjigaId == id && z.Status == "aktivno");
        }

        public async Task<IEnumerable<Knjiga>> GetRandomAsync(int count)
        {
            if (count <= 0) return Array.Empty<Knjiga>();

            var total = await _db.Knjige.CountAsync();
            if (total == 0) return Array.Empty<Knjiga>();

            var take = Math.Min(count, total);
            if (take == total)
            {
                var all = await _db.Knjige
                    .Include(k => k.Kategorija)
                    .AsNoTracking()
                    .ToListAsync();
                for (int i = all.Count - 1; i > 0; i--)
                {
                    int j = Random.Shared.Next(i + 1);
                    (all[i], all[j]) = (all[j], all[i]);
                }
                return all;
            }

            var minId = await _db.Knjige.MinAsync(k => k.Id);
            var maxId = await _db.Knjige.MaxAsync(k => k.Id);
            if (minId == maxId)
            {
                return await _db.Knjige
                    .Include(k => k.Kategorija)
                    .AsNoTracking()
                    .Where(k => k.Id == minId)
                    .ToListAsync();
            }

            var upperBound = maxId == int.MaxValue ? maxId : maxId + 1;
            var startId = Random.Shared.Next(minId, upperBound);
            var firstBatch = await _db.Knjige
                .Include(k => k.Kategorija)
                .AsNoTracking()
                .Where(k => k.Id >= startId)
                .OrderBy(k => k.Id)
                .Take(take)
                .ToListAsync();

            if (firstBatch.Count < take)
            {
                var remaining = take - firstBatch.Count;
                var secondBatch = await _db.Knjige
                    .Include(k => k.Kategorija)
                    .AsNoTracking()
                    .Where(k => k.Id < startId)
                    .OrderBy(k => k.Id)
                    .Take(remaining)
                    .ToListAsync();
                firstBatch.AddRange(secondBatch);
            }

            return firstBatch.OrderBy(_ => Random.Shared.Next()).ToList();
        }

        public async Task<bool> TryUpdateOpisByIsbnAsync(string isbn, string opis)
        {
            if (string.IsNullOrWhiteSpace(isbn) || string.IsNullOrWhiteSpace(opis)) return false;

            var knjiga = await _db.Knjige.FirstOrDefaultAsync(k => k.Isbn == isbn);
            if (knjiga == null) return false;
            if (!string.IsNullOrWhiteSpace(knjiga.Opis)) return false;

            knjiga.Opis = opis.Trim();
            await _db.SaveChangesAsync();
            return true;
        }
    }
}