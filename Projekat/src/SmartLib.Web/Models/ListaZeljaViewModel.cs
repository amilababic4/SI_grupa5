namespace SmartLib.Web.Models
{
    public class ListaZeljaViewModel
    {
        public int OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public bool IsOwner { get; set; }
        public bool IsPublic { get; set; }
        public string Sort { get; set; } = "added";
        public List<ListaZeljaItemViewModel> Items { get; set; } = new();
    }

    public class ListaZeljaItemViewModel
    {
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
