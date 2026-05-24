using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class VijestRepository : IVijestRepository
    {
        private readonly ApplicationDbContext _db;
        public VijestRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Vijest>> GetAllAsync()
            => await _db.Vijesti
                .Include(v => v.Autor)
                .OrderByDescending(v => v.DatumObjave)
                .ToListAsync();

        public async Task<Vijest?> GetByIdAsync(int id)
            => await _db.Vijesti
                .Include(v => v.Autor)
                .FirstOrDefaultAsync(v => v.Id == id);

        public async Task<Vijest> CreateAsync(Vijest vijest)
        {
            _db.Vijesti.Add(vijest);
            await _db.SaveChangesAsync();
            return vijest;
        }

        public async Task UpdateAsync(Vijest vijest)
        {
            _db.Vijesti.Update(vijest);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var vijest = await _db.Vijesti.FindAsync(id);
            if (vijest != null)
            {
                _db.Vijesti.Remove(vijest);
                await _db.SaveChangesAsync();
            }
        }
    }
}
