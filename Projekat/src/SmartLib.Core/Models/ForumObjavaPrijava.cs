namespace SmartLib.Core.Models
{
    /// <summary>
    /// Prijava neadekvatne forum objave
    /// </summary>
    public class ForumObjavaPrijava
    {
        public int Id { get; set; }
        public int ObjavaId { get; set; }
        public ForumObjava? Objava { get; set; }
        public int PrijavioKorisnikId { get; set; }
        public Korisnik? PrijavioKorisnik { get; set; }
        public string? Razlog { get; set; }
        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "otvorena";
        public int? RazrijesioKorisnikId { get; set; }
        public Korisnik? RazrijesioKorisnik { get; set; }
        public DateTime? DatumRazrjesenja { get; set; }
    }
}
