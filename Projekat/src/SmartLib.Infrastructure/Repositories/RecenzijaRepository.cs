using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class RecenzijaRepository : IRecenzijaRepository
    {
        private readonly ApplicationDbContext _db;

        public RecenzijaRepository(ApplicationDbContext db) => _db = db;

        public async Task<Recenzija> AddAsync(Recenzija recenzija)
        {
            _db.Recenzije.Add(recenzija);
            await _db.SaveChangesAsync();
            return recenzija;
        }

        public async Task<IEnumerable<Recenzija>> GetByKnjigaIdAsync(int knjigaId)
        {
            return await _db.Recenzije
                .Include(r => r.Korisnik).ThenInclude(k => k!.Uloga)
                .Where(r => r.KnjigaId == knjigaId)
                .OrderByDescending(r => r.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<bool> HasUserReviewedAsync(int knjigaId, int korisnikId)
        {
            return await _db.Recenzije.AnyAsync(r => r.KnjigaId == knjigaId && r.KorisnikId == korisnikId);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var recenzija = await _db.Recenzije.FirstOrDefaultAsync(r => r.Id == id);
            if (recenzija == null)
            {
                return false;
            }

            _db.Recenzije.Remove(recenzija);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
