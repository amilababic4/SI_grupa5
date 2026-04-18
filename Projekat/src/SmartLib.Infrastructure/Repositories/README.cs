// TODO: Implementirati repozitorije
// Svaki repozitorij implementira odgovarajući interfejs iz SmartLib.Core.Interfaces
// i koristi ApplicationDbContext za pristup bazi podataka.
//
// Primjer strukture:
//
// using SmartLib.Core.Interfaces;
// using SmartLib.Core.Models;
// using SmartLib.Infrastructure.Data;
// using Microsoft.EntityFrameworkCore;
//
// namespace SmartLib.Infrastructure.Repositories
// {
//     public class KorisnikRepository : IKorisnikRepository
//     {
//         private readonly ApplicationDbContext _context;
//
//         public KorisnikRepository(ApplicationDbContext context)
//         {
//             _context = context;
//         }
//
//         public async Task<IEnumerable<Korisnik>> GetAllAsync()
//         {
//             return await _context.Korisnici
//                 .Include(k => k.Uloga)
//                 .Where(k => k.Status == "aktivan")
//                 .ToListAsync();
//         }
//
//         // ... ostale metode
//     }
// }
