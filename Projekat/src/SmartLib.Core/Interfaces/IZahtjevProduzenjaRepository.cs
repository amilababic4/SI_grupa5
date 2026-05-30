// Lokacija: SmartLib.Core/Interfaces/IZahtjevProduzenjaRepository.cs
// NOVI FAJL — dodati u projekt.

using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IZahtjevProduzenjaRepository
    {
        // ── Član ─────────────────────────────────────────────────────────

        /// <summary>
        /// Vraća aktivni zahtjev na čekanju za određenog korisnika, ako postoji.
        /// Koristi se da se spriječi višestruko slanje zahtjeva.
        /// </summary>
        Task<ZahtjevProduzenja?> GetAktivniZahtjevAsync(int korisnikId);

        /// <summary>
        /// Vraća historiju svih zahtjeva određenog korisnika (sortirano: najnoviji prvo).
        /// </summary>
        Task<IEnumerable<ZahtjevProduzenja>> GetZahtjeviByKorisnikAsync(int korisnikId);

        /// <summary>Kreira novi zahtjev za produženje.</summary>
        Task<ZahtjevProduzenja> CreateAsync(ZahtjevProduzenja zahtjev);

        // ── Bibliotekar / Administrator ───────────────────────────────────

        /// <summary>
        /// Vraća sve zahtjeve sa statusom "na_cekanju" (lista za obradu).
        /// </summary>
        Task<IEnumerable<ZahtjevProduzenja>> GetNaCekanjuAsync();

        /// <summary>
        /// Vraća zahtjev po ID-u sa učitanim navigacionim svojstvima.
        /// </summary>
        Task<ZahtjevProduzenja?> GetByIdAsync(int id);

        /// <summary>Ažurira zahtjev (status, razlog, datum obrade).</summary>
        Task UpdateAsync(ZahtjevProduzenja zahtjev);

        /// <summary>
        /// Vraća broj zahtjeva na čekanju — za badge/notifikaciju u UI.
        /// </summary>
        Task<int> BrojNaCekanjuAsync();
    }
}