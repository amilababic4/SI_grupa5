using System.ComponentModel.DataAnnotations;

namespace SmartLib.Core.DTOs
{
    public class ZaduzenjeCreateDto
    {
        [Required(ErrorMessage = "Odaberite člana.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite člana.")]
        public int KorisnikId { get; set; }

        // Koristi se samo u UI za filtriranje primjeraka - ne sprema se direktno
        [Required(ErrorMessage = "Odaberite knjigu.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite knjigu.")]
        public int KnjigaId { get; set; }

        [Required(ErrorMessage = "Odaberite primjerak.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite primjerak.")]
        public int PrimjerakId { get; set; }

        // Opcionalni rok vracanja koji unosi bibliotekar; ako nije unesen, generira se automatski
        [DataType(DataType.Date)]
        public DateTime? DatumPovratka { get; set; }
    }

    public class ZaduzenjeViewModel
    {
        public int Id { get; set; }
        public int KnjigaId { get; set; }
        public string KorisnikIme { get; set; } = string.Empty;
        public string KorisnikEmail { get; set; } = string.Empty;
        public string KnjigaNaslov { get; set; } = string.Empty;
        public string InventarniBroj { get; set; } = string.Empty;
        public DateTime DatumZaduzivanja { get; set; }
        public DateTime DatumPlaniranogVracanja { get; set; }
        public DateTime? DatumStvarnogVracanja { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool JeZakasnilo { get; set; }
        public bool RokSeBliži { get; set; }
    }

    public class AktivnaZaduzenjaViewModel
    {
        public IEnumerable<ZaduzenjeViewModel> Zaduzenja { get; set; } = new List<ZaduzenjeViewModel>();
        public string? Clan { get; set; }
    }

    public class HistorijaZaduzenjaViewModel
    {
        public string? Clan { get; set; }
        public List<ZaduzenjeViewModel> Zaduzenja { get; set; } = new List<ZaduzenjeViewModel>();
    }
}
