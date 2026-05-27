namespace SmartLib.Core.Models
{
    public class ListaZeljaStavka
    {
        public int Id { get; set; }
        public int KorisnikId { get; set; }
        public int KnjigaId { get; set; }
        public DateTime DatumDodavanja { get; set; } = DateTime.UtcNow;

        public Korisnik? Korisnik { get; set; }
        public Knjiga? Knjiga { get; set; }
    }
}
