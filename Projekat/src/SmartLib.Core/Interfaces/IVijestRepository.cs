using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IVijestRepository
    {
        Task<IEnumerable<Vijest>> GetAllAsync();
        Task<Vijest?> GetByIdAsync(int id);
        Task<Vijest> CreateAsync(Vijest vijest);
        Task UpdateAsync(Vijest vijest);
        Task DeleteAsync(int id);
    }
}
