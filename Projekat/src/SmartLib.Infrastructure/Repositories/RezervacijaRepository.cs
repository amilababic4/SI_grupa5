using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class RezervacijaRepository : IRezervacijaRepository
    {
        private readonly ApplicationDbContext _db;

        public RezervacijaRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Rezervacija>> GetActiveAsync()
        {
            await CancelExpiredAsync();
            return await _db.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.Knjiga)
                    .ThenInclude(k => k!.Primjerci)
                .Where(r => r.Status == "aktivna")
                .OrderBy(r => r.DatumRezervacije)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rezervacija>> GetByKorisnikAsync(int korisnikId)
        {
            await CancelExpiredAsync();
            return await _db.Rezervacije
                .Include(r => r.Knjiga)
                    .ThenInclude(k => k!.Primjerci)
                .Where(r => r.KorisnikId == korisnikId && r.Status == "aktivna")
                .OrderBy(r => r.DatumRezervacije)
                .ToListAsync();
        }

        public async Task<Rezervacija?> GetByIdAsync(int id)
        {
            return await _db.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.Knjiga)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Rezervacija?> GetNextActiveForBookAsync(int knjigaId)
        {
            await CancelExpiredAsync();
            return await _db.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.Knjiga)
                .Where(r => r.KnjigaId == knjigaId && r.Status == "aktivna")
                .OrderBy(r => r.DatumRezervacije)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasActiveAsync(int korisnikId, int knjigaId)
        {
            return await _db.Rezervacije
                .AnyAsync(r => r.KorisnikId == korisnikId
                            && r.KnjigaId == knjigaId
                            && r.Status == "aktivna");
        }

        public async Task<Rezervacija> CreateAsync(Rezervacija rezervacija)
        {
            _db.Rezervacije.Add(rezervacija);
            await _db.SaveChangesAsync();
            return rezervacija;
        }

        public async Task UpdateAsync(Rezervacija rezervacija)
        {
            _db.Rezervacije.Update(rezervacija);
            await _db.SaveChangesAsync();
        }

        public async Task FulfillAsync(int korisnikId, int knjigaId)
        {
            var rezervacija = await _db.Rezervacije
                .FirstOrDefaultAsync(r => r.KorisnikId == korisnikId
                                       && r.KnjigaId == knjigaId
                                       && r.Status == "aktivna");
            if (rezervacija == null) return;

            rezervacija.Status = "realizovana";
            await _db.SaveChangesAsync();
        }

        public async Task CancelExpiredAsync()
        {
            var istekle = await _db.Rezervacije
                .Where(r => r.Status == "aktivna" && r.DatumIsteka < DateTime.UtcNow)
                .ToListAsync();

            if (istekle.Count == 0) return;

            foreach (var r in istekle)
                r.Status = "istekla";

            await _db.SaveChangesAsync();
        }
    }
}
