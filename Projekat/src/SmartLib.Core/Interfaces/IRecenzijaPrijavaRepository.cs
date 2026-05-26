using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IRecenzijaPrijavaRepository
    {
        Task<RecenzijaPrijava> AddAsync(RecenzijaPrijava prijava);
        Task<bool> HasUserReportedAsync(int recenzijaId, int korisnikId);
        Task<List<int>> GetReportedRecenzijaIdsAsync(int korisnikId, int knjigaId);
        Task<List<RecenzijaPrijava>> GetOpenAsync();
        Task<RecenzijaPrijava?> GetByIdAsync(int id);
        Task<bool> TryResolveAsync(int prijavaId, int resolverId);
    }
}
