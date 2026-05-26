using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IRecenzijaRepository
    {
        Task<Recenzija> AddAsync(Recenzija recenzija);
        Task<Recenzija?> GetByIdAsync(int id);
        Task<IEnumerable<Recenzija>> GetByKnjigaIdAsync(int knjigaId);
        Task<bool> HasUserReviewedAsync(int knjigaId, int korisnikId);
        Task<bool> DeleteAsync(int id);
    }
}
