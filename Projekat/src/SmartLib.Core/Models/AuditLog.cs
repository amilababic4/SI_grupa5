namespace SmartLib.Core.Models
{
    /// <summary>
    /// Audit trail zapisa svih kritičnih promjena u sistemu.
    /// Vrijednosti prije/nakon su JSONB polja.
    /// </summary>
    public class AuditLog
    {
        public int Id { get; set; }
        public int? KorisnikId { get; set; }
        public string Akcija { get; set; } = string.Empty;
        public string EntitetTip { get; set; } = string.Empty;
        public int? EntitetId { get; set; }
        public string? VrijednostiPrije { get; set; }   // JSON string (JSONB u bazi)
        public string? VrijednostiNakon { get; set; }   // JSON string (JSONB u bazi)
        public DateTime DatumAkcije { get; set; } = DateTime.UtcNow;

        // Navigaciono svojstvo
        public Korisnik? Korisnik { get; set; }
    }
}
