using System.ComponentModel.DataAnnotations;

namespace SmartLib.Core.DTOs
{
    public class ForumObjavaDto
    {
        public int Id { get; set; }
        public string Naslov { get; set; } = string.Empty;
        public string Sadrzaj { get; set; } = string.Empty;
        public string Kategorija { get; set; } = string.Empty;
        public DateTime DatumKreiranja { get; set; }
        public bool Zakljucana { get; set; }
        public string AutorIme { get; set; } = string.Empty;
        public string AutorUloga { get; set; } = string.Empty;
        public int KorisnikId { get; set; }
        public int BrojKomentara { get; set; }
        public int BrojReakcija { get; set; }
        public bool KorisnikJeReagovao { get; set; }
        public List<ForumKomentarDto> Komentari { get; set; } = new();
    }

    public class ForumKomentarDto
    {
        public int Id { get; set; }
        public string Sadrzaj { get; set; } = string.Empty;
        public DateTime DatumKreiranja { get; set; }
        public string AutorIme { get; set; } = string.Empty;
        public string AutorUloga { get; set; } = string.Empty;
        public int KorisnikId { get; set; }
    }

    public class ForumObjavaCreateDto
    {
        [Required(ErrorMessage = "Naslov je obavezan.")]
        [MaxLength(200, ErrorMessage = "Naslov ne smije biti duži od 200 znakova.")]
        public string Naslov { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sadržaj je obavezan.")]
        [MaxLength(5000, ErrorMessage = "Sadržaj ne smije biti duži od 5000 znakova.")]
        public string Sadrzaj { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategorija je obavezna.")]
        public string Kategorija { get; set; } = "Opšta diskusija";
    }

    public class ForumKomentarCreateDto
    {
        public int ObjavaId { get; set; }

        [Required(ErrorMessage = "Komentar ne može biti prazan.")]
        [MaxLength(2000, ErrorMessage = "Komentar ne smije biti duži od 2000 znakova.")]
        public string Sadrzaj { get; set; } = string.Empty;
    }

    public class ForumIndexViewModel
    {
        public List<ForumObjavaDto> Objave { get; set; } = new();
        public string? AktivnaKategorija { get; set; }
        public List<string> Kategorije { get; set; } = new();
    }
}
