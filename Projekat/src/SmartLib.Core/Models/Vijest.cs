namespace SmartLib.Core.Models
{
    public class Vijest
    {
        public int Id { get; set; }
        public string Naslov { get; set; } = string.Empty;
        public string Sadrzaj { get; set; } = string.Empty;
        public string Kategorija { get; set; } = "Obavještenje";
        public string? SlikaUrl { get; set; }
        public DateTime DatumObjave { get; set; } = DateTime.UtcNow;
        public int AutorId { get; set; }
        public Korisnik? Autor { get; set; }
    }
}
