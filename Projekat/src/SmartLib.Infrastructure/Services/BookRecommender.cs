using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SmartLib.Infrastructure.Services
{
    public class BookRecommender : IBookRecommender
    {
        private readonly ApplicationDbContext _dbContext;

        public BookRecommender(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Knjiga>> GetRecommendationsForUserAsync(int korisnikId)
        {
            // TODO: Implement actual recommendation logic based on user history, ratings, and categories.
            // Requires adding `Ocjene` (Ratings) and `IstorijaCitanja` (Reading History) models to `Korisnik`.

            // Placeholder logic: Return top 4 popular or recently added books.
            return await _dbContext.Knjige
                .OrderByDescending(k => k.Id) // Fallback to newly added books
                .Take(4)
                .ToListAsync();
        }
    }
}
