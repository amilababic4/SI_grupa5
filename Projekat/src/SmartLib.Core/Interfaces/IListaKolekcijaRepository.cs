using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IListaKolekcijaRepository
    {
        Task<ListaKolekcija> EnsureWishlistCollectionAsync(int korisnikId);
        Task<List<ListaKolekcija>> GetByKorisnikAsync(int korisnikId);
        Task<ListaKolekcija?> GetByIdAsync(int id, bool includeItems = false);
        Task<ListaKolekcija> CreateAsync(ListaKolekcija lista);
        Task UpdateAsync(ListaKolekcija lista);
        Task<bool> DeleteAsync(int id, int korisnikId);
        Task<bool> HasItemAsync(int listaId, int knjigaId);
        Task AddItemAsync(int listaId, int knjigaId);
        Task RemoveItemAsync(int listaId, int knjigaId);
        Task UpdateOrderAsync(int listaId, IReadOnlyList<int> stavkaIds);
    }
}
