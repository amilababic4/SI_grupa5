namespace SmartLib.Core.Models
{
    public class Notifikacija
    {
        public int Id { get; set; }
        public int KorisnikId { get; set; }
        public Korisnik? Korisnik { get; set; }
        public string Naslov { get; set; } = string.Empty;
        public string Poruka { get; set; } = string.Empty;
        public string Tip { get; set; } = "Sistem";
        public string? LinkUrl { get; set; }
        public bool Procitano { get; set; }
        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
    }
}
