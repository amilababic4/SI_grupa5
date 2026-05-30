using Microsoft.EntityFrameworkCore;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;

namespace SmartLib.Infrastructure.Repositories;

public class NabavkaRepository : INabavkaRepository
{
    private readonly ApplicationDbContext _db;
    public NabavkaRepository(ApplicationDbContext db) => _db = db;

    public async Task CreateAsync(NabavkaZahtjev zahtjev)
    {
        _db.NabavkaZahtjevi.Add(zahtjev);
        await _db.SaveChangesAsync();
    }

    public async Task<List<NabavkaZahtjev>> GetZadnjeAsync(int broj = 3)
        => await _db.NabavkaZahtjevi
            .OrderByDescending(z => z.VrijemePodnosenja)
            .Take(broj)
            .ToListAsync();

    public async Task<string?> GetDistributerEmailAsync()
        => await _db.AppPostavke
            .Where(p => p.Kljuc == "distributer_email")
            .Select(p => p.Vrijednost)
            .FirstOrDefaultAsync();

    public async Task SetDistributerEmailAsync(string email)
    {
        var p = await _db.AppPostavke
            .FirstOrDefaultAsync(x => x.Kljuc == "distributer_email");
        if (p is null)
            _db.AppPostavke.Add(new AppPostavka { Kljuc = "distributer_email", Vrijednost = email });
        else
            p.Vrijednost = email;
        await _db.SaveChangesAsync();
    }
}