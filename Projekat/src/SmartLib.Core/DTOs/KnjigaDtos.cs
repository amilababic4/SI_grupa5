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
        public string Naslov { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public int KategorijaId { get; set; }
        public string? Izdavac { get; set; }
        public int? GodinaIzdanja { get; set; }
        public int BrojPrimjeraka { get; set; } = 1;
    }
}
