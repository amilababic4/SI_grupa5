namespace SmartLib.Core.Models
{
    /// <summary>
    /// Forum objava koju može kreirati registrovani korisnik (PB-57)
    /// Kategorije: Opšta diskusija / Preporuke knjiga / Pitanja / Recenzije (PB-59)
    /// </summary>
    public class ForumObjava
    {
        public int Id { get; set; }
        public string Naslov { get; set; } = string.Empty;
        public string Sadrzaj { get; set; } = string.Empty;
        public string Kategorija { get; set; } = "Opšta diskusija";
        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
        public bool Zakljucana { get; set; } = false;

        public int KorisnikId { get; set; }
        public Korisnik? Korisnik { get; set; }

        public ICollection<ForumKomentar> Komentari { get; set; } = new List<ForumKomentar>();
        public ICollection<ForumReakcija> Reakcije { get; set; } = new List<ForumReakcija>();
    }
}
