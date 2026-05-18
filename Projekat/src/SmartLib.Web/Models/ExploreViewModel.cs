using System.Collections.Generic;

namespace SmartLib.Web.Models
{
    public class ExploreViewModel
    {
        public List<ExploreCardViewModel> Cards { get; set; } = new();
        public List<string> PreferredCategories { get; set; } = new();
        public List<string> SurpriseCategories { get; set; } = new();
    }

    public class ExploreCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string InfoLink { get; set; } = string.Empty;
        public bool IsWildcard { get; set; }
    }
}
