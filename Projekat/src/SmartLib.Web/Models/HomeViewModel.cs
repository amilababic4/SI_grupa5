using SmartLib.Core.DTOs;

namespace SmartLib.Web.Models
{
    public class HomeViewModel
    {
        public List<KnjigaDto> RandomKnjige { get; set; } = new();
        public List<KnjigaDto> RecommendedKnjige { get; set; } = new();
        public List<HomeVijestDto> RecentVijesti { get; set; } = new();
        public List<HomeDogadjajDto> UpcomingDogadjaji { get; set; } = new();
    }

    public sealed class HomeVijestDto
    {
        public int Id { get; set; }
        public string Naslov { get; set; } = string.Empty;
        public string Sadrzaj { get; set; } = string.Empty;
        public string Kategorija { get; set; } = string.Empty;
        public string? SlikaUrl { get; set; }
        public DateTime DatumObjave { get; set; }
    }

    public sealed class HomeDogadjajDto
    {
        public int Id { get; set; }
        public string Naslov { get; set; } = string.Empty;
        public string Kategorija { get; set; } = string.Empty;
        public DateTime Datum { get; set; }
        public string? Sat { get; set; }
        public string? Lokacija { get; set; }
    }
}
