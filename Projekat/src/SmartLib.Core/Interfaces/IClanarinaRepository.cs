using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IClanarinaRepository
    {
        Task<Clanarina?> GetByKorisnikAsync(int korisnikId);
        Task<Clanarina> CreateAsync(Clanarina clanarina);
        Task UpdateAsync(Clanarina clanarina);
        Task<bool> IsActiveAsync(int korisnikId);
    }
}
