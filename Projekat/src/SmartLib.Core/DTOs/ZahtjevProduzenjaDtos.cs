// Lokacija: SmartLib.Core/DTOs/ZahtjevProduzenjaDtos.cs
// NOVI FAJL — dodati u projekt.

using System.ComponentModel.DataAnnotations;

namespace SmartLib.Core.DTOs
{
    // ── 1. DTO za podnošenje zahtjeva (član → server) ─────────────────────

    /// <summary>
    /// Podaci koje član šalje kad podnosi zahtjev za produženje.
    /// </summary>
    public class ZahtjevProduzenjaCreateDto
    {
        [Required(ErrorMessage = "Trajanje je obavezno.")]
        [Range(1, 12, ErrorMessage = "Trajanje mora biti između 1 i 12 mjeseci.")]
        public int TrajanjeMjeseci { get; set; }

        [StringLength(500, ErrorMessage = "Napomena ne smije biti duža od 500 znakova.")]
        public string? Napomena { get; set; }
    }

    // ── 2. DTO za prikaz zahtjeva (server → view) ────────────────────────

    /// <summary>
    /// Podaci za prikaz jednog zahtjeva (koristi se i na strani člana i bibliotekara).
    /// </summary>
    public class ZahtjevProduzenjaDto
    {
        public int Id { get; set; }
        public int KorisnikId { get; set; }
        public string ImeClana { get; set; } = string.Empty;
        public string PrezimeClana { get; set; } = string.Empty;
        public string EmailClana { get; set; } = string.Empty;
        public int TrajanjeMjeseci { get; set; }
        public string? Napomena { get; set; }
        public string Status { get; set; } = "na_cekanju";
        public DateTime DatumPodnosenja { get; set; }
        public DateTime? DatumObrade { get; set; }
        public string? RazlogOdbijanja { get; set; }
        public DateTime? NoviDatumIsteka { get; set; }

        // Tekuća članarina — prikazuje se bibliotekaru radi konteksta
        public DateTime? TrenutniDatumIsteka { get; set; }
        public string? TrenutniStatus { get; set; }

        // Derivirani izračun — procijenjeni novi datum (za preview na formi člana)
        public DateTime? ProcijenjeniNoviDatumIsteka { get; set; }
    }

    // ── 3. DTO za obradu zahtjeva (bibliotekar → server) ─────────────────

    /// <summary>
    /// Podaci koje bibliotekar šalje pri odobravanju ili odbijanju.
    /// </summary>
    public class ZahtjevProduzenjaObradiDto
    {
        [Required]
        public int ZahtjevId { get; set; }

        /// <summary>
        /// "odobreno" ili "odbijeno"
        /// </summary>
        [Required(ErrorMessage = "Akcija je obavezna.")]
        public string Akcija { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Razlog ne smije biti duži od 500 znakova.")]
        public string? RazlogOdbijanja { get; set; }
    }

    // ── 4. ViewModel za stranicu člana ───────────────────────────────────

    /// <summary>
    /// Sve što treba View za stranicu produženja sa strane člana.
    /// </summary>
    public class ProduzenjeViewModel
    {
        public int KorisnikId { get; set; }

        // Tekuća članarina
        public DateTime? TrenutniDatumIsteka { get; set; }
        public string? TrenutniStatus { get; set; }
        public bool ImaClanarinu { get; set; }

        // Aktivni zahtjev na čekanju (ako postoji)
        public ZahtjevProduzenjaDto? AktivniZahtjev { get; set; }
        public bool ImaAktivniZahtjev => AktivniZahtjev is not null;

        // Historija zahtjeva
        public IEnumerable<ZahtjevProduzenjaDto> Historija { get; set; } = [];

        // Form model za novi zahtjev
        public ZahtjevProduzenjaCreateDto NoviZahtjev { get; set; } = new();
    }

    // ── 5. ViewModel za stranicu bibliotekara ────────────────────────────

    /// <summary>
    /// Sve što treba View za pregled i obradu zahtjeva (strana bibliotekara).
    /// </summary>
    public class ZahtjeviProduzenjaViewModel
    {
        public IEnumerable<ZahtjevProduzenjaDto> NaCekanju { get; set; } = [];
        public int UkupnoNaCekanju => NaCekanju.Count();
    }
}