using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IRezervacijaRepository
    {
        Task<IEnumerable<Rezervacija>> GetActiveAsync();
        Task<IEnumerable<Rezervacija>> GetByKorisnikAsync(int korisnikId);
        Task<Rezervacija?> GetByIdAsync(int id);
        Task<Rezervacija?> GetNextActiveForBookAsync(int knjigaId);
        Task<bool> HasActiveAsync(int korisnikId, int knjigaId);
        Task<Rezervacija> CreateAsync(Rezervacija rezervacija);
        Task UpdateAsync(Rezervacija rezervacija);
        Task CancelExpiredAsync();
        Task FulfillAsync(int korisnikId, int knjigaId);
    }
}
