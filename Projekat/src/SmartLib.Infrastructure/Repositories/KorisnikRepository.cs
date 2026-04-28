using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class KorisnikRepository : IKorisnikRepository
    {
        private readonly ApplicationDbContext _db;

        public KorisnikRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Korisnik>> GetAllAsync()
        {
            return await _db.Korisnici
                .Include(k => k.Uloga)
                .Where(k => k.Status == "aktivan")
                .ToListAsync();
        }

        public async Task<Korisnik?> GetByIdAsync(int id)
        {
            return await _db.Korisnici
                .Include(k => k.Uloga)
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<Korisnik?> GetByEmailAsync(string email)
        {
            var normalized = email.Trim().ToLowerInvariant();
            return await _db.Korisnici
                .Include(k => k.Uloga)
                .FirstOrDefaultAsync(k => k.Email == normalized);
        }

        public async Task<Korisnik> CreateAsync(Korisnik korisnik)
        {
            _db.Korisnici.Add(korisnik);
            await _db.SaveChangesAsync();
            return korisnik;
        }

        public async Task UpdateAsync(Korisnik korisnik)
        {
            _db.Korisnici.Update(korisnik);
            await _db.SaveChangesAsync();
        }
    }
}