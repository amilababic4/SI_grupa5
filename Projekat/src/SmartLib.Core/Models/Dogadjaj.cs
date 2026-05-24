namespace SmartLib.Core.Models
{
    public class Dogadjaj
    {
        public int Id { get; set; }
        public string Naslov { get; set; } = string.Empty;
        public string? Opis { get; set; }
        public DateTime Datum { get; set; }
        public string? Sat { get; set; }
        public string? Lokacija { get; set; }
        public string Kategorija { get; set; } = "Edukacija";
        public int AutorId { get; set; }
        public Korisnik? Autor { get; set; }
    }
}
