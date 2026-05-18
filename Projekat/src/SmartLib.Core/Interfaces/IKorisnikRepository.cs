using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    /// <summary>
    /// Repozitorij za upravljanje korisnicima
    /// </summary>
    public interface IKorisnikRepository
    {
        Task<IEnumerable<Korisnik>> GetAllAsync();
        Task<Korisnik?> GetByIdAsync(int id);
        Task<Korisnik?> GetByEmailAsync(string email);
        Task<Korisnik> CreateAsync(Korisnik korisnik);
        Task UpdateAsync(Korisnik korisnik);
        Task<Korisnik?> GetByResetTokenAsync(string token);
        Task<int> DeleteDeactivatedOlderThanAsync(DateTime cutoffUtc);
        // Nema DeleteAsync — koristimo soft delete (deaktivacija)
    }
}
