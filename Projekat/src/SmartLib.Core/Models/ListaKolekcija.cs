namespace SmartLib.Core.Models
{
    public class ListaKolekcija
    {
        public int Id { get; set; }
        public int KorisnikId { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string? Opis { get; set; }
        public bool Javna { get; set; } = false;
        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;
        public DateTime? DatumAzuriranja { get; set; }

        public Korisnik? Korisnik { get; set; }
        public ICollection<ListaKolekcijaStavka> Stavke { get; set; } = new List<ListaKolekcijaStavka>();
    }
}
