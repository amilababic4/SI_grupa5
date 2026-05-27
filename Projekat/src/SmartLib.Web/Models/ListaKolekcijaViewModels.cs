namespace SmartLib.Web.Models
{
    public class ListaKolekcijaListViewModel
    {
        public List<ListaKolekcijaCardViewModel> Kolekcije { get; set; } = new();
        public string? NewNaziv { get; set; }
        public string? NewOpis { get; set; }
        public bool NewJavna { get; set; }
        public string? Query { get; set; }
        public string Visibility { get; set; } = "all";
        public string Sort { get; set; } = "updated";
        public int TotalCount { get; set; }
    }

    public class ListaKolekcijaCardViewModel
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string? Opis { get; set; }
        public bool Javna { get; set; }
        public int BrojStavki { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public DateTime? DatumAzuriranja { get; set; }
        public bool IsWishlist { get; set; }
    }

    public class ListaKolekcijaDetailsViewModel
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string? Opis { get; set; }
        public bool Javna { get; set; }
        public bool IsOwner { get; set; }
        public List<ListaKolekcijaItemViewModel> Stavke { get; set; } = new();
    }

    public class ListaKolekcijaItemViewModel
    {
        public int StavkaId { get; set; }
        public int KnjigaId { get; set; }
        public string Naslov { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public int? GodinaIzdanja { get; set; }
        public DateTime DatumDodavanja { get; set; }
        public string? SlikaUrl { get; set; }
        public int BrojDostupnih { get; set; }
    }
}
