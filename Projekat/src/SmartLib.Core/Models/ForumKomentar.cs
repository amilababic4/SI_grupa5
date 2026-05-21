namespace SmartLib.Core.Models
{
    /// <summary>
    /// Komentar na forum objavu (PB-58)
    /// </summary>
    public class ForumKomentar
    {
        public int Id { get; set; }
        public string Sadrzaj { get; set; } = string.Empty;
        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;

        public int ObjavaId { get; set; }
        public ForumObjava? Objava { get; set; }

        public int KorisnikId { get; set; }
        public Korisnik? Korisnik { get; set; }
    }
}
