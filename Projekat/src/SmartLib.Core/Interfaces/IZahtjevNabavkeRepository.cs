using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IZahtjevNabavkeRepository
    {
        Task<ZahtjevNabavke> CreateAsync(ZahtjevNabavke zahtjevNabavke);
    }
}
