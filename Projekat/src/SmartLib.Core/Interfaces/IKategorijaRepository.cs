using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IKategorijaRepository
    {
        Task<IEnumerable<Kategorija>> GetAllAsync();
        Task<Kategorija?> GetByIdAsync(int id);
        Task<Kategorija> CreateAsync(Kategorija kategorija);
        Task UpdateAsync(Kategorija kategorija);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasBooksAsync(int id);
    }
}
