using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IZaduzenjeRepository
    {
        Task<IEnumerable<Zaduzenje>> GetActiveAsync();
        Task<IEnumerable<Zaduzenje>> GetByKorisnikAsync(int korisnikId);
        Task<IEnumerable<Zaduzenje>> GetHistoryByKorisnikAsync(int korisnikId);
        Task<Zaduzenje?> GetByIdAsync(int id);
        Task<Zaduzenje> CreateAsync(Zaduzenje zaduzenje);
        Task UpdateAsync(Zaduzenje zaduzenje);
        Task<IEnumerable<Zaduzenje>> GetClosedSinceAsync(DateTime granica);
        Task<IEnumerable<Zaduzenje>> GetClosedHistoryForKorisnikAsync(int korisnikId, DateTime granica);
        Task<IEnumerable<Zaduzenje>> GetByPrimjerakAsync(int primjerakId);
        Task<int> CountByKnjigaIdAsync(int knjigaId);
        Task<IEnumerable<Zaduzenje>> GetAktivnaZaDatumAsync(DateTime datum);
        Task<IEnumerable<Zaduzenje>> GetKasnaAktivnaAsync(DateTime danas);
        Task<IEnumerable<Zaduzenje>> GetAktivnaZaDatumRangeAsync(DateTime od, DateTime doo);
        Task<bool> ImaKasnelaZaduzenjaAsync(int korisnikId);
    }
}