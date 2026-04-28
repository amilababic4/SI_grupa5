using System.ComponentModel.DataAnnotations.Schema;

namespace SmartLib.Core.Models
{
    /// <summary>
    /// Evidencija zaduživanja/vraćanja knjiga.
    /// Status: aktivno / zatvoreno / zakašnjelo
    /// </summary>
    public class Zaduzenje
    {
        public int Id { get; set; }
        public int KorisnikId { get; set; }
        public int PrimjerakId { get; set; }
        public DateTime DatumZaduzivanja { get; set; }
        public DateTime DatumPlaniranogVracanja { get; set; }
        public DateTime? DatumStvarnogVracanja { get; set; }
        public string Status { get; set; } = "aktivno";   // aktivno / zatvoreno / zakašnjelo

        // Navigaciona svojstva
        public Korisnik? Korisnik { get; set; }
        public Primjerak? Primjerak { get; set; }
    }
}
