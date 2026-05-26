using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface INotifikacijaRepository
    {
        Task AddAsync(Notifikacija notifikacija);
        Task AddRangeAsync(IEnumerable<Notifikacija> notifikacije);
        Task<List<Notifikacija>> GetForUserAsync(int korisnikId);
        Task<int> GetUnreadCountAsync(int korisnikId);
        Task MarkReadAsync(int notifikacijaId, int korisnikId);
        Task MarkAllReadAsync(int korisnikId);
        Task<bool> HasRecentAsync(int korisnikId, string tip, string? linkUrl, TimeSpan window);
    }
}
