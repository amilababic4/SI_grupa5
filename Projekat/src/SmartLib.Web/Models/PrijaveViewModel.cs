namespace SmartLib.Web.Models
{
    public class PrijaveViewModel
    {
        public List<PrijavaListItem> Stavke { get; set; } = new();
    }

    public class PrijavaListItem
    {
        public string Tip { get; set; } = string.Empty;
        public string Naslov { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Prijavio { get; set; } = string.Empty;
        public string Razlog { get; set; } = string.Empty;
        public DateTime Datum { get; set; }
        public string? LinkUrl { get; set; }
        public int PrijavaId { get; set; }
        public string Source { get; set; } = string.Empty;
    }
}
