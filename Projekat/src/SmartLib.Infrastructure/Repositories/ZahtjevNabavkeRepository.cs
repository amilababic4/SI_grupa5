using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories
{
    public class ZahtjevNabavkeRepository : IZahtjevNabavkeRepository
    {
        private readonly ApplicationDbContext _db;

        public ZahtjevNabavkeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ZahtjevNabavke> CreateAsync(ZahtjevNabavke zahtjevNabavke)
        {
            _db.Set<ZahtjevNabavke>().Add(zahtjevNabavke);
            await _db.SaveChangesAsync();
            return zahtjevNabavke;
        }
    }
}
