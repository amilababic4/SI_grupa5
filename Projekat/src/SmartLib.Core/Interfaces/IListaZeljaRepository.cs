using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IListaZeljaRepository
    {
        Task<List<ListaZeljaStavka>> GetByKorisnikAsync(int korisnikId);
        Task<bool> ExistsAsync(int korisnikId, int knjigaId);
        Task AddAsync(int korisnikId, int knjigaId);
        Task RemoveAsync(int korisnikId, int knjigaId);
    }
}
