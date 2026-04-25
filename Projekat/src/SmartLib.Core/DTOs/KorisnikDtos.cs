using System.ComponentModel.DataAnnotations;

namespace SmartLib.Core.DTOs
{
    public class KorisnikDto
    {
        public int Id { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Uloga { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DatumKreiranja { get; set; }
    }

    public class KorisnikCreateDto
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        [StringLength(100, ErrorMessage = "Ime ne smije imati više od 100 znakova.")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [StringLength(100, ErrorMessage = "Prezime ne smije imati više od 100 znakova.")]
        public string Prezime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email adresa je obavezna.")]
        [EmailAddress(ErrorMessage = "Email adresa nije u ispravnom formatu.")]
        [StringLength(256, ErrorMessage = "Email adresa ne smije imati više od 256 znakova.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [MinLength(8, ErrorMessage = "Lozinka mora imati najmanje 8 znakova.")]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = string.Empty;
    }
}
