namespace SmartLib.Core.Models
{
    /// <summary>
    /// Prijava neadekvatnog komentara na forumu (PB-63)
    /// </summary>
    public class ForumKomentarPrijava
    {
        public int Id { get; set; }
        public int KomentarId { get; set; }
        public ForumKomentar? Komentar { get; set; }
        public int PrijavioKorisnikId { get; set; }
        public Korisnik? PrijavioKorisnik { get; set; }
        public string? Razlog { get; set; }
        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
    }
}
