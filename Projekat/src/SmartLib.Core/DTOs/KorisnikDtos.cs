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
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Lozinka { get; set; } = string.Empty;
    }
}
