using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class DogadjajRepository : IDogadjajRepository
    {
        private readonly ApplicationDbContext _db;
        public DogadjajRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Dogadjaj>> GetAllAsync()
            => await _db.Dogadjaji
                .Include(d => d.Autor)
                .OrderBy(d => d.Datum)
                .ToListAsync();

        public async Task<Dogadjaj?> GetByIdAsync(int id)
            => await _db.Dogadjaji
                .Include(d => d.Autor)
                .FirstOrDefaultAsync(d => d.Id == id);

        public async Task<Dogadjaj> CreateAsync(Dogadjaj dogadjaj)
        {
            _db.Dogadjaji.Add(dogadjaj);
            await _db.SaveChangesAsync();
            return dogadjaj;
        }

        public async Task UpdateAsync(Dogadjaj dogadjaj)
        {
            _db.Dogadjaji.Update(dogadjaj);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var dogadjaj = await _db.Dogadjaji.FindAsync(id);
            if (dogadjaj != null)
            {
                _db.Dogadjaji.Remove(dogadjaj);
                await _db.SaveChangesAsync();
            }
        }
    }
}
