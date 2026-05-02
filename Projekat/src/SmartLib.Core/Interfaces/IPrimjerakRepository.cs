using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IPrimjerakRepository
    {
        Task<IEnumerable<Primjerak>> GetByKnjigaAsync(int knjigaId);
        Task<Primjerak?> GetByIdAsync(int id);
        Task<Primjerak> CreateAsync(Primjerak primjerak);
        Task UpdateStatusAsync(int id, string status);
        Task<int> GetAvailableCountAsync(int knjigaId);

        // US-24: Deaktivacija primjerka
        Task<bool> HasActiveZaduzenjeAsync(int primjerakId);
        Task DeactivateAsync(int id);
    }
}