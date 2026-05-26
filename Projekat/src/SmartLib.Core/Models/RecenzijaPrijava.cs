namespace SmartLib.Core.Models
{
    /// <summary>
    /// Prijava neadekvatne recenzije (moderacija)
    /// </summary>
    public class RecenzijaPrijava
    {
        public int Id { get; set; }
        public int RecenzijaId { get; set; }
        public Recenzija? Recenzija { get; set; }
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
