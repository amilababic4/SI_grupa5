namespace SmartLib.Core.Models
{
    /// <summary>
    /// Reakcija korisnika na forum objavu – "korisno" toggle (PB-60)
    /// </summary>
    public class ForumReakcija
    {
        public int Id { get; set; }
        public string Tip { get; set; } = "korisno";
        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;

        public int ObjavaId { get; set; }
        public ForumObjava? Objava { get; set; }

        public int KorisnikId { get; set; }
        public Korisnik? Korisnik { get; set; }
    }
}
