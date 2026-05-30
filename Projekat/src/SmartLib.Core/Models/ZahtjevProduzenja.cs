// Lokacija: SmartLib.Core/Models/ZahtjevProduzenja.cs
// NOVI FAJL — dodati u projekt.

namespace SmartLib.Core.Models
{
    /// <summary>
    /// Zahtjev člana za online produženje članarine (PB-48).
    /// Workflow: član podnosi → bibliotekar odobrava/odbija → datum isteka se ažurira.
    /// </summary>
    public class ZahtjevProduzenja
    {
        public int Id { get; set; }

        /// <summary>FK na korisnika koji podnosi zahtjev.</summary>
        public int KorisnikId { get; set; }

        /// <summary>
        /// Traženo trajanje produženja u mjesecima.
        /// Dozvoljene vrijednosti: 1, 3, 6, 12.
        /// </summary>
        public int TrajanjeMjeseci { get; set; }

        /// <summary>
        /// Opcionalna napomena člana uz zahtjev.
        /// </summary>
        public string? Napomena { get; set; }

        /// <summary>
        /// Status zahtjeva: "na_cekanju" | "odobreno" | "odbijeno"
        /// </summary>
        public string Status { get; set; } = "na_cekanju";

        /// <summary>Datum i vrijeme podnošenja zahtjeva (UTC).</summary>
        public DateTime DatumPodnosenja { get; set; } = DateTime.UtcNow;

        /// <summary>Datum i vrijeme obrade od strane bibliotekara (UTC).</summary>
        public DateTime? DatumObrade { get; set; }

        /// <summary>FK na bibliotekara koji je obradio zahtjev (nullable).</summary>
        public int? ObradioKorisnikId { get; set; }

        /// <summary>Razlog odbijanja — popunjava bibliotekar samo kad Status = "odbijeno".</summary>
        public string? RazlogOdbijanja { get; set; }

        /// <summary>
        /// Novi datum isteka koji je sistem izračunao/postavio pri odobravanju.
        /// Pohranjuje se radi historijskog traga.
        /// </summary>
        public DateTime? NoviDatumIsteka { get; set; }

        // ── Navigaciona svojstva ──────────────────────────────────────────
        public Korisnik? Korisnik { get; set; }
        public Korisnik? ObradioKorisnik { get; set; }
    }
}