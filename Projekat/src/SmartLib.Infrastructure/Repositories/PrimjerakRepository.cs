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

        public async Task<Primjerak> CreateAsync(Primjerak primjerak)
        {
            _db.Primjerci.Add(primjerak);
            await _db.SaveChangesAsync();
            return primjerak;
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var primjerak = await _db.Primjerci.FindAsync(id);
            if (primjerak != null)
            {
                primjerak.Status = status;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<int> GetAvailableCountAsync(int knjigaId)
        {
            return await _db.Primjerci
                .CountAsync(p => p.KnjigaId == knjigaId && p.Status == "dostupan");
        }
    }
}
