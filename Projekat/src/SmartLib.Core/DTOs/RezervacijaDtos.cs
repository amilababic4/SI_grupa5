using System.ComponentModel.DataAnnotations;

namespace SmartLib.Core.DTOs
{
    public class RezervacijaViewModel
    {
        public int Id { get; set; }
        public string KnjigaNaslov { get; set; } = string.Empty;
        public string KorisnikIme { get; set; } = string.Empty;
        public string KorisnikEmail { get; set; } = string.Empty;
        public DateTime DatumRezervacije { get; set; }
        public DateTime? DatumIsteka { get; set; }
        public string Status { get; set; } = string.Empty;
        // True when the reserved book now has at least one available copy
        public bool KnjigaDostupna { get; set; }
    }

    public class AktivneRezervacijeViewModel
    {
        public IEnumerable<RezervacijaViewModel> Rezervacije { get; set; } = new List<RezervacijaViewModel>();
        public string? Filter { get; set; }
    }

    public class RezervacijaCreateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Nevažeći ID knjige.")]
        public int KnjigaId { get; set; }
    }
}
