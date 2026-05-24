using SmartLib.Core.DTOs;

namespace SmartLib.Core.Interfaces
{
    public interface IIzvjestajService
    {
        Task<MjesecniZaduzenjaIzvjestajDto> GenerirajMjesecnaZaduzenja(int mjesec, int godina);
        Task<MjesecneRezervacijeIzvjestajDto> GenerirajMjesecneRezervacije(int mjesec, int godina);
        Task<MjesecniClanoviIzvjestajDto> GenerirajMjesecneKlanove(int mjesec, int godina);
    }
}