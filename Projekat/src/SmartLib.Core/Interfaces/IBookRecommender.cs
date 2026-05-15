using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IBookRecommender
    {
        Task<IEnumerable<Knjiga>> GetRecommendationsForUserAsync(int korisnikId);
    }
}
