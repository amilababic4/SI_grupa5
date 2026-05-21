namespace SmartLib.Core.Models
{
    public class ZahtjevNabavke
    {
        public int Id { get; set; }
        public string NaslovKnjige { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Izdavac { get; set; } = string.Empty;
        public int BrojPrimjeraka { get; set; }
        public string? Napomena { get; set; }
        public string DistributorEmail { get; set; } = string.Empty;
        public string Status { get; set; } = "poslano";
        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
        public DateTime? DatumSlanja { get; set; }
        public int? BibliotekarId { get; set; }
    }
}
