using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class KorisnikRepository : IKorisnikRepository
    {
        public Task<IEnumerable<Korisnik>> GetAllAsync()
        {
            var korisnici = SmartLibDataStore.GetKorisnici()
                .Where(k => string.Equals(k.Status, "aktivan", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Task.FromResult<IEnumerable<Korisnik>>(korisnici);
        }

        public Task<Korisnik?> GetByIdAsync(int id)
        {
            return Task.FromResult(SmartLibDataStore.GetKorisnikById(id));
        }

        public Task<Korisnik?> GetByEmailAsync(string email)
        {
            return Task.FromResult(SmartLibDataStore.GetKorisnikByEmail(email));
        }

        public Task<Korisnik> CreateAsync(Korisnik korisnik)
        {
            var created = SmartLibDataStore.AddKorisnik(korisnik);
            return Task.FromResult(created);
        }

        public Task UpdateAsync(Korisnik korisnik)
        {
            SmartLibDataStore.UpdateKorisnik(korisnik);
            return Task.CompletedTask;
        }
    }
}