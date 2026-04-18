namespace SmartLib.Core.DTOs
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
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
