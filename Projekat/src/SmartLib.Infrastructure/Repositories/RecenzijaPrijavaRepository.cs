using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class RecenzijaPrijavaRepository : IRecenzijaPrijavaRepository
    {
        private readonly ApplicationDbContext _db;

        public RecenzijaPrijavaRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<RecenzijaPrijava> AddAsync(RecenzijaPrijava prijava)
        {
            _db.RecenzijaPrijave.Add(prijava);
            await _db.SaveChangesAsync();
            return prijava;
        }

        public async Task<bool> HasUserReportedAsync(int recenzijaId, int korisnikId)
        {
            return await _db.RecenzijaPrijave
                .AnyAsync(p => p.RecenzijaId == recenzijaId && p.PrijavioKorisnikId == korisnikId);
        }

        public async Task<List<int>> GetReportedRecenzijaIdsAsync(int korisnikId, int knjigaId)
        {
            return await _db.RecenzijaPrijave
                .Where(p => p.PrijavioKorisnikId == korisnikId && p.Recenzija!.KnjigaId == knjigaId)
                .Select(p => p.RecenzijaId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<RecenzijaPrijava>> GetOpenAsync()
        {
            return await _db.RecenzijaPrijave
                .Include(p => p.PrijavioKorisnik)
                .Include(p => p.RazrijesioKorisnik)
                .Include(p => p.Recenzija)
                    .ThenInclude(r => r!.Knjiga)
                .Include(p => p.Recenzija)
                    .ThenInclude(r => r!.Korisnik)
                .Where(p => p.Status == "otvorena")
                .OrderBy(p => p.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<RecenzijaPrijava?> GetByIdAsync(int id)
        {
            return await _db.RecenzijaPrijave
                .Include(p => p.PrijavioKorisnik)
                .Include(p => p.RazrijesioKorisnik)
                .Include(p => p.Recenzija)
                    .ThenInclude(r => r!.Knjiga)
                .Include(p => p.Recenzija)
                    .ThenInclude(r => r!.Korisnik)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> TryResolveAsync(int prijavaId, int resolverId)
        {
            var affected = await _db.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE RecenzijaPrijave
                SET Status = {"razrijesena"},
                    RazrijesioKorisnikId = {resolverId},
                    DatumRazrjesenja = {DateTime.UtcNow}
                WHERE Id = {prijavaId} AND Status = {"otvorena"}
            ");

            return affected > 0;
        }
    }
}
