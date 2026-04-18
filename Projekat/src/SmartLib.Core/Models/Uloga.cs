namespace SmartLib.Core.Models
{
    /// <summary>
    /// Uloga korisnika u sistemu: Član, Bibliotekar, Administrator
    /// </summary>
    public class Uloga
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;  // UNIQUE
        public string? Opis { get; set; }

        // Navigaciono svojstvo
        public ICollection<Korisnik> Korisnici { get; set; } = new List<Korisnik>();
    }
}
