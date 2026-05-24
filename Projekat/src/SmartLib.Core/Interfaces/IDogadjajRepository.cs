using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IDogadjajRepository
    {
        Task<IEnumerable<Dogadjaj>> GetAllAsync();
        Task<Dogadjaj?> GetByIdAsync(int id);
        Task<Dogadjaj> CreateAsync(Dogadjaj dogadjaj);
        Task UpdateAsync(Dogadjaj dogadjaj);
        Task DeleteAsync(int id);
    }
}
