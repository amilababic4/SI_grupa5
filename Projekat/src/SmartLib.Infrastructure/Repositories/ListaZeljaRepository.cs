using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class ListaZeljaRepository : IListaZeljaRepository
    {
        private readonly ApplicationDbContext _db;

        public ListaZeljaRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<ListaZeljaStavka>> GetByKorisnikAsync(int korisnikId)
        {
            return await _db.ListaZeljaStavke
                .Include(l => l.Knjiga)
                    .ThenInclude(k => k!.Primjerci)
                .Where(l => l.KorisnikId == korisnikId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int korisnikId, int knjigaId)
        {
            return await _db.ListaZeljaStavke
                .AnyAsync(l => l.KorisnikId == korisnikId && l.KnjigaId == knjigaId);
        }

        public async Task AddAsync(int korisnikId, int knjigaId)
        {
            if (await ExistsAsync(korisnikId, knjigaId)) return;

            _db.ListaZeljaStavke.Add(new ListaZeljaStavka
            {
                KorisnikId = korisnikId,
                KnjigaId = knjigaId,
                DatumDodavanja = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }

        public async Task RemoveAsync(int korisnikId, int knjigaId)
        {
            var entity = await _db.ListaZeljaStavke
                .FirstOrDefaultAsync(l => l.KorisnikId == korisnikId && l.KnjigaId == knjigaId);
            if (entity == null) return;

            _db.ListaZeljaStavke.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
