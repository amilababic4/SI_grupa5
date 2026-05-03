using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class KategorijaRepository : IKategorijaRepository
    {
        private readonly ApplicationDbContext _db;

        public KategorijaRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        // Include(Knjige) da bi Count radio u viewu i API-ju
        public async Task<IEnumerable<Kategorija>> GetAllAsync()
        {
            return await _db.Kategorije
                .Include(k => k.Knjige)
                .OrderBy(k => k.Naziv)
                .ToListAsync();
        }

        public async Task<Kategorija?> GetByIdAsync(int id)
        {
            return await _db.Kategorije
                .Include(k => k.Knjige)
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<Kategorija> CreateAsync(Kategorija kategorija)
        {
            _db.Kategorije.Add(kategorija);
            await _db.SaveChangesAsync();
            return kategorija;
        }

        public async Task UpdateAsync(Kategorija kategorija)
        {
            _db.Kategorije.Update(kategorija);
            await _db.SaveChangesAsync();
        }

        // US-32 + US-34: Vraæa false ako kategorija ima knjige
        public async Task<bool> DeleteAsync(int id)
        {
            var kategorija = await _db.Kategorije.FindAsync(id);
            if (kategorija == null) return false;
            if (await HasBooksAsync(id)) return false;

            _db.Kategorije.Remove(kategorija);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasBooksAsync(int id)
        {
            return await _db.Knjige.AnyAsync(k => k.KategorijaId == id);
        }
    }
}