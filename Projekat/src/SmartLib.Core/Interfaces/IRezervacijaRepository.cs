using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IRezervacijaRepository
    {
        Task<IEnumerable<Rezervacija>> GetActiveAsync();
        Task<IEnumerable<Rezervacija>> GetByKorisnikAsync(int korisnikId);
        Task<Rezervacija> CreateAsync(Rezervacija rezervacija);
        Task UpdateAsync(Rezervacija rezervacija);
    }
}
