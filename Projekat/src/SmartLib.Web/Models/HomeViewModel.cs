using SmartLib.Core.DTOs;
using SmartLib.Core.Models;

namespace SmartLib.Web.Models
{
    public class HomeViewModel
    {
        public List<KnjigaDto> RandomKnjige { get; set; } = new();
        public List<KnjigaDto> RecommendedKnjige { get; set; } = new();
        public List<Vijest> RecentVijesti { get; set; } = new();
        public List<Dogadjaj> UpcomingDogadjaji { get; set; } = new();
    }
}
