namespace SmartLib.Core.Models
{
    /// <summary>
    /// Fizički primjerak knjige sa jedinstvenim inventarnim brojem.
    /// Status: dostupan / zadužen
    /// </summary>
    public class Primjerak
    {
        public int Id { get; set; }
        public int KnjigaId { get; set; }
        public string InventarniBroj { get; set; } = string.Empty;  // UNIQUE
        public string Status { get; set; } = "dostupan";            // dostupan / zadužen
        public DateTime? DatumNabave { get; set; }

        // Navigaciona svojstva
        public Knjiga? Knjiga { get; set; }
        public ICollection<Zaduzenje> Zaduzenja { get; set; } = new List<Zaduzenje>();
    }
}
