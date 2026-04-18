namespace SmartLib.Core.Models
{
    /// <summary>
    /// Kataloški (bibliografski) zapis knjige.
    /// Predstavlja logički opis, ne fizički primjerak. ISBN je jedinstven.
    /// </summary>
    public class Knjiga
    {
        public int Id { get; set; }
        public string Naslov { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;    // UNIQUE
        public int KategorijaId { get; set; }
        public string? Izdavac { get; set; }
        public int? GodinaIzdanja { get; set; }

        // Navigaciona svojstva
        public Kategorija? Kategorija { get; set; }
        public ICollection<Primjerak> Primjerci { get; set; } = new List<Primjerak>();
        public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();
    }
}
