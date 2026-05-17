// Lokacija: SmartLib.Infrastructure/Repositories/ClanarinaRepository.cs

using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class ClanarinaRepository : IClanarinaRepository
    {
        private readonly ApplicationDbContext _context;

        public ClanarinaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Vraća najnoviju članarinu za datog korisnika (po datumu isteka, desc).
        /// </summary>
        public async Task<Clanarina?> GetByKorisnikAsync(int korisnikId)
            => await _context.Clanarine
                .Where(c => c.KorisnikId == korisnikId)
                .OrderByDescending(c => c.DatumIsteka)
                .FirstOrDefaultAsync();

        public async Task<Clanarina> CreateAsync(Clanarina clanarina)
        {
            // Deriviraj status pri kreiranju
            clanarina.Status = DateTime.UtcNow.Date <= clanarina.DatumIsteka.Date
                ? "aktivna" : "istekla";

            _context.Clanarine.Add(clanarina);
            await _context.SaveChangesAsync();
            return clanarina;
        }

        public async Task UpdateAsync(Clanarina clanarina)
        {
            // Deriviraj status pri ažuriranju
            clanarina.Status = DateTime.UtcNow.Date <= clanarina.DatumIsteka.Date
                ? "aktivna" : "istekla";

            _context.Clanarine.Update(clanarina);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Provjerava ima li korisnik aktivnu članarinu (datum isteka u budućnosti).
        /// </summary>
        public async Task<bool> IsActiveAsync(int korisnikId)
            => await _context.Clanarine
                .AnyAsync(c => c.KorisnikId == korisnikId
                            && c.DatumIsteka.Date >= DateTime.UtcNow.Date);
    }
}