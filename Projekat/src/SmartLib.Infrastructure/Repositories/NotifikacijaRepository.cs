using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class NotifikacijaRepository : INotifikacijaRepository
    {
        private readonly ApplicationDbContext _db;

        public NotifikacijaRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Notifikacija notifikacija)
        {
            _db.Notifikacije.Add(notifikacija);
            await _db.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Notifikacija> notifikacije)
        {
            _db.Notifikacije.AddRange(notifikacije);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Notifikacija>> GetForUserAsync(int korisnikId)
        {
            return await _db.Notifikacije
                .Where(n => n.KorisnikId == korisnikId)
                .OrderByDescending(n => n.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int korisnikId)
        {
            return await _db.Notifikacije
                .CountAsync(n => n.KorisnikId == korisnikId && !n.Procitano);
        }

        public async Task MarkReadAsync(int notifikacijaId, int korisnikId)
        {
            var notif = await _db.Notifikacije
                .FirstOrDefaultAsync(n => n.Id == notifikacijaId && n.KorisnikId == korisnikId);
            if (notif == null) return;

            notif.Procitano = true;
            await _db.SaveChangesAsync();
        }

        public async Task MarkAllReadAsync(int korisnikId)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE Notifikacije
                SET Procitano = 1
                WHERE KorisnikId = {korisnikId} AND Procitano = 0
            ");
        }

        public async Task<bool> HasRecentAsync(int korisnikId, string tip, string? linkUrl, TimeSpan window)
        {
            var cutoff = DateTime.UtcNow.Subtract(window);
            return await _db.Notifikacije.AnyAsync(n =>
                n.KorisnikId == korisnikId &&
                n.Tip == tip &&
                n.LinkUrl == linkUrl &&
                n.DatumKreiranja >= cutoff);
        }
    }
}
