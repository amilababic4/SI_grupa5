using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    /// <summary>
    /// Repozitorij za upravljanje knjigama (katalog)
    /// </summary>
    public interface IKnjigaRepository
    {
        Task<IEnumerable<Knjiga>> GetAllAsync(int page, int pageSize);
        Task<Knjiga?> GetByIdAsync(int id);
        Task<IEnumerable<Knjiga>> SearchAsync(string? naslov, string? autor);
        Task<Knjiga> CreateAsync(Knjiga knjiga);
        Task UpdateAsync(Knjiga knjiga);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasActiveLoansAsync(int id);
    }
}
