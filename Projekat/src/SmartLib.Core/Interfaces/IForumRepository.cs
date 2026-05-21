using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    /// <summary>
    /// Repository interface za forum funkcionalnost (PB-57, PB-58, PB-59, PB-60)
    /// </summary>
    public interface IForumRepository
    {
        /// <summary>PB-59 – Dohvati sve objave, opcionalno filtrirano po kategoriji</summary>
        Task<IEnumerable<ForumObjava>> GetAllAsync(string? kategorija = null);

        /// <summary>PB-57 – Dohvati jednu objavu sa komentarima i reakcijama</summary>
        Task<ForumObjava?> GetByIdAsync(int id);

        /// <summary>PB-57 – Kreiraj novu forum objavu</summary>
        Task<ForumObjava> CreateAsync(ForumObjava objava);

        /// <summary>PB-58 – Dodaj komentar na objavu</summary>
        Task<ForumKomentar> AddKomentarAsync(ForumKomentar komentar);

        /// <summary>Ukloni komentar sa objave</summary>
        Task<bool> DeleteKomentarAsync(int komentarId);

        /// <summary>PB-60 – Toggle "korisno" reakcija; vraća true ako je dodana, false ako je uklonjena</summary>
        Task<bool> ToggleReakcijaAsync(int objavaId, int korisnikId, string tip = "korisno");

        /// <summary>PB-60 – Provjeri da li korisnik već ima reakciju na objavu</summary>
        Task<bool> HasReakcijaAsync(int objavaId, int korisnikId, string tip = "korisno");

        /// <summary>Dohvati dostupne kategorije</summary>
        IReadOnlyList<string> GetKategorije();
    }
}
