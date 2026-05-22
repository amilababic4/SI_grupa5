using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IKnjigaRepository
    {
        Task<IEnumerable<Knjiga>> GetAllAsync(int page, int pageSize);
        Task<Knjiga?> GetByIdAsync(int id);
        Task<IEnumerable<Knjiga>> SearchAsync(string? naslov, string? autor);
        Task<Knjiga> CreateAsync(Knjiga knjiga);
        Task UpdateAsync(Knjiga knjiga);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasActiveLoansAsync(int id);

        /// <summary>
        /// PB-44: Proširena paginirana pretraga sa filterima po kategoriji, izdavaèu i godini.
        /// </summary>
        Task<(IEnumerable<Knjiga> Knjige, int UkupnoBroj)> GetPagedAsync(
            string? naslov,
            string? autor,
            int page,
            int pageSize,
            int? kategorijaId = null,
            string? izdavac = null,
            int? godinaIzdanja = null);

        Task<Knjiga?> GetByIsbnAsync(string isbn);
        Task<IEnumerable<Knjiga>> GetRandomAsync(int count);
        Task<bool> TryUpdateOpisByIsbnAsync(string isbn, string opis);

        /// <summary>
        /// PB-44: Dohvata sve dostupne izdavaèe za filter dropdown.
        /// </summary>
        Task<IEnumerable<string>> GetDistinctIzdavaciAsync();

        /// <summary>
        /// PB-44: Dohvata sve dostupne godine izdanja za filter dropdown.
        /// </summary>
        Task<IEnumerable<int>> GetDistinctGodineAsync();
    }
}