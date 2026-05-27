using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class ZaduzenjeRepository : IZaduzenjeRepository
    {
        private readonly ApplicationDbContext _db;

        public ZaduzenjeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

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

        public async Task<Zaduzenje> CreateAsync(Zaduzenje zaduzenje)
        {
            _db.Zaduzenja.Add(zaduzenje);
            await _db.SaveChangesAsync();
            return zaduzenje;
        }

        public async Task UpdateAsync(Zaduzenje zaduzenje)
        {
            _db.Zaduzenja.Update(zaduzenje);
            await _db.SaveChangesAsync();
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

        public async Task<int> CountByKnjigaIdAsync(int knjigaId)
        {
            return await _db.Zaduzenja
                .Include(z => z.Primjerak)
                .Where(z => z.Primjerak!.KnjigaId == knjigaId)
                .CountAsync();
        }

    }
}
