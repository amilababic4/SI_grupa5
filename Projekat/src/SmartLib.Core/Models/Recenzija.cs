namespace SmartLib.Core.Models
{
    public class Recenzija
    {
        public int Id { get; set; }
        public int KnjigaId { get; set; }
        public Knjiga? Knjiga { get; set; }
        public int KorisnikId { get; set; }
        public Korisnik? Korisnik { get; set; }
        public int Ocjena { get; set; } // 1 do 5
        public string Komentar { get; set; } = string.Empty;
        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
    }
}
