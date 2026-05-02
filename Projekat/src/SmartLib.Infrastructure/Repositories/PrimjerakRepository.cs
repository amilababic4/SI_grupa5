using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class PrimjerakRepository : IPrimjerakRepository
    {
        private readonly ApplicationDbContext _db;

        public PrimjerakRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        // US-22, US-23: Pregled svih primjeraka knjige sa statusima
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

        // US-21: Kreiranje primjerka pri dodavanju knjige
        public async Task<Primjerak> CreateAsync(Primjerak primjerak)
        {
            _db.Primjerci.Add(primjerak);
            await _db.SaveChangesAsync();
            return primjerak;
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var primjerak = await _db.Primjerci.FindAsync(id);
            if (primjerak == null) return;

            primjerak.Status = status;
            await _db.SaveChangesAsync();
        }

        public async Task<int> GetAvailableCountAsync(int knjigaId)
        {
            return await _db.Primjerci
                .CountAsync(p => p.KnjigaId == knjigaId && p.Status == "dostupan");
        }

        // US-24: Provjera ima li primjerak aktivno zaduženje
        public async Task<bool> HasActiveZaduzenjeAsync(int primjerakId)
        {
            return await _db.Zaduzenja
                .AnyAsync(z => z.PrimjerakId == primjerakId && z.Status == "aktivno");
        }

        // US-24: Deaktivacija primjerka (status -> "deaktiviran")
        public async Task DeactivateAsync(int id)
        {
            var primjerak = await _db.Primjerci.FindAsync(id);
            if (primjerak == null) return;

            primjerak.Status = "deaktiviran";
            await _db.SaveChangesAsync();
        }
    }
}