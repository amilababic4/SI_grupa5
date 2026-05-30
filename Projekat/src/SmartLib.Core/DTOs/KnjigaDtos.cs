using SmartLib.Core.Models;
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
        public string? Opis { get; set; }
        public int BrojPrimjeraka { get; set; }
        public int BrojDostupnih { get; set; }
        public double ProsjecnaOcjena { get; set; }
        public int BrojRecenzija { get; set; }
        public bool Procitana { get; set; }
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

    /// <summary>
    /// PB-44: Prošireni ViewModel kataloga sa filterima napredne pretrage.
    /// </summary>
    public class KatalogViewModel
    {
        public IEnumerable<KnjigaDto> Knjige { get; set; } = new List<KnjigaDto>();
        public int TrenutnaStrana { get; set; }
        public int UkupnoStrana { get; set; }
        public int UkupnoStavki { get; set; }
        public int VelicinaStrane { get; set; }

        public bool SamoNeprocitane { get; set; }
        public int ProcitaneUkupno { get; set; }

        // Osnovna pretraga
        public string? Naslov { get; set; }
        public string? Autor { get; set; }

        // PB-44: Dodatni filteri
        public int? KategorijaId { get; set; }
        public string? Izdavac { get; set; }
        public int? GodinaIzdanja { get; set; }

        // PB-44: Podaci za dropdown listu filtera
        public IEnumerable<KategorijaFilterDto> Kategorije { get; set; } = new List<KategorijaFilterDto>();
        public IEnumerable<string> Izdavaci { get; set; } = new List<string>();
        public IEnumerable<int> Godine { get; set; } = new List<int>();

        /// <summary>
        /// Vraća true ako je aktivan bilo koji filter (osnovna pretraga ili napredni filteri).
        /// </summary>
        public bool ImaAktivnihFiltera =>
            !string.IsNullOrWhiteSpace(Naslov) ||
            !string.IsNullOrWhiteSpace(Autor) ||
            KategorijaId.HasValue ||
            !string.IsNullOrWhiteSpace(Izdavac) ||
            GodinaIzdanja.HasValue ||
            SamoNeprocitane;

        /// <summary>
        /// Vraća true ako je aktivan makar jedan napredni filter (ne osnovna pretraga).
        /// Koristi se za automatsko otvaranje panela pri page loadu.
        /// </summary>
        public bool ImaNapredniFilter =>
            KategorijaId.HasValue ||
            !string.IsNullOrWhiteSpace(Izdavac) ||
            GodinaIzdanja.HasValue ||
            SamoNeprocitane;
    }

    /// <summary>
    /// PB-44: Minimalni DTO za prikaz kategorija u filter dropdownu.
    /// </summary>
    public class KategorijaFilterDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
    }

    public class NabavkaZahtjevDto
    {
        [Required(ErrorMessage = "Naziv knjige je obavezan.")]
        public string NazivKnjige { get; set; } = string.Empty;

        [Required(ErrorMessage = "Autor je obavezan.")]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "Izdavač je obavezan.")]
        public string Izdavac { get; set; } = string.Empty;

        [Required, Range(1, 999, ErrorMessage = "Broj primjeraka mora biti između 1 i 999.")]
        public int BrojPrimjeraka { get; set; }

        public string? Napomena { get; set; }
    }

    public class NabavkaPageViewModel
    {
        public NabavkaZahtjevDto Forma { get; set; } = new();
        public List<NabavkaZahtjev> ZadnjeNabavke { get; set; } = new();
        public string DistributerEmail { get; set; } = string.Empty;
    }

    public class PromijeniDistributerDto
    {
        [Required(ErrorMessage = "Email adresa je obavezna.")]
        [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
        public string Email { get; set; } = string.Empty;
    }
}