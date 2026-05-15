using SmartLib.Core.DTOs;

namespace SmartLib.Web.Models
{
    public class HomeViewModel
    {
        public List<KnjigaDto> RandomKnjige { get; set; } = new();
        public List<KnjigaDto> RecommendedKnjige { get; set; } = new();
    }
}
