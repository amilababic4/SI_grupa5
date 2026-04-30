using System.ComponentModel.DataAnnotations;

namespace SmartLib.Core.DTOs
{
    public class KnjigaDto
    {
        public int Id { get; set; }
        public string Naslov { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public string? Kategorija { get; set; }
        public string? Izdavac { get; set; }
        public int? GodinaIzdanja { get; set; }
        public int BrojPrimjeraka { get; set; }
        public int BrojDostupnih { get; set; }
    }

    public class KnjigaCreateDto
    {
        [Required(ErrorMessage = "Naslov je obavezan.")]
        [StringLength(300, ErrorMessage = "Naslov ne smije imati više od 300 znakova.")]
        public string Naslov { get; set; } = string.Empty;

        [Required(ErrorMessage = "Autor je obavezan.")]
        [StringLength(200, ErrorMessage = "Autor ne smije imati više od 200 znakova.")]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN je obavezan.")]
        [StringLength(20, ErrorMessage = "ISBN ne smije imati više od 20 znakova.")]
        public string Isbn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategorija je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite kategoriju.")]
        public int KategorijaId { get; set; }

        [StringLength(200, ErrorMessage = "Izdavač ne smije imati više od 200 znakova.")]
        public string? Izdavac { get; set; }

        [Required(ErrorMessage = "Godina izdanja je obavezna.")]
        [Range(1000, 2100, ErrorMessage = "Godina izdanja mora biti između 1000 i 2100.")]
        public int? GodinaIzdanja { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Broj primjeraka mora biti nenegativan cijeli broj.")]
        public int BrojPrimjeraka { get; set; } = 1;
    }

    public class KnjigaEditDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naslov je obavezan.")]
        [StringLength(300, ErrorMessage = "Naslov ne smije imati više od 300 znakova.")]
        public string Naslov { get; set; } = string.Empty;

        [Required(ErrorMessage = "Autor je obavezan.")]
        [StringLength(200, ErrorMessage = "Autor ne smije imati više od 200 znakova.")]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategorija je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite kategoriju.")]
        public int KategorijaId { get; set; }

        [StringLength(200, ErrorMessage = "Izdavač ne smije imati više od 200 znakova.")]
        public string? Izdavac { get; set; }

        [Required(ErrorMessage = "Godina izdanja je obavezna.")]
        [Range(1000, 2100, ErrorMessage = "Godina izdanja mora biti između 1000 i 2100.")]
        public int? GodinaIzdanja { get; set; }
    }

    public class KatalogViewModel
    {
        public IEnumerable<KnjigaDto> Knjige { get; set; } = new List<KnjigaDto>();
        public int TrenutnaStrana { get; set; }
        public int UkupnoStrana { get; set; }
        public int UkupnoStavki { get; set; }
        public int VelicinaStrane { get; set; }
        public string? Naslov { get; set; }
        public string? Autor { get; set; }
    }
}
