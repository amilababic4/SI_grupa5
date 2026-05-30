using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces;

public interface INabavkaRepository
{
    Task CreateAsync(NabavkaZahtjev zahtjev);
    Task<List<NabavkaZahtjev>> GetZadnjeAsync(int broj = 3);
    Task<string?> GetDistributerEmailAsync();
    Task SetDistributerEmailAsync(string email);
}