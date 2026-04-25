using System.ComponentModel.DataAnnotations;

namespace SmartLib.Core.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email adresa je obavezna.")]
        [EmailAddress(ErrorMessage = "Email adresa nije u ispravnom formatu.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Uloga { get; set; } = string.Empty;
    }
}
