namespace SmartLib.Core.DTOs
{
    public class MjesecniIzvjestajFilterDto
    {
        public int Mjesec { get; set; }
        public int Godina { get; set; }
    }

    public class ZaduzenjeIzvjestajRedDto
    {
        public int RedniBroj { get; set; }
        public string ClanImePrezime { get; set; } = string.Empty;
        public string ClanEmail { get; set; } = string.Empty;
        public string NaslovKnjige { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string InventarniBroj { get; set; } = string.Empty;
        public DateTime DatumZaduzivanja { get; set; }
        public DateTime? DatumPlaniranogVracanja { get; set; }
        public DateTime? DatumStvarnogVracanja { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class MjesecniZaduzenjaIzvjestajDto
    {
        public int Mjesec { get; set; }
        public int Godina { get; set; }
        public string NazivMjeseca { get; set; } = string.Empty;
        public int UkupnoZaduzenja { get; set; }
        public int AktivnaZaduzenja { get; set; }
        public int ZatvorenaZaduzenja { get; set; }
        public int ZakasnjelaZaduzenja { get; set; }
        public List<ZaduzenjeIzvjestajRedDto> Stavke { get; set; } = new();
        public DateTime GenerisanoU { get; set; } = DateTime.Now;

        public List<int> ZaduzenjaPoDanima { get; set; } = new();
        public List<KnjigaRankDto> TopKnjige { get; set; } = new();

    }

    // ── US-89: Rezervacije ──────────────────────────────────────────────────────

    public class RezervacijaIzvjestajRedDto
    {
        public int RedniBroj { get; set; }
        public string ClanImePrezime { get; set; } = string.Empty;
        public string ClanEmail { get; set; } = string.Empty;
        public string NaslovKnjige { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public DateTime DatumRezervacije { get; set; }
        public DateTime? DatumIsteka { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class MjesecneRezervacijeIzvjestajDto
    {
        public int Mjesec { get; set; }
        public int Godina { get; set; }
        public string NazivMjeseca { get; set; } = string.Empty;
        public int UkupnoRezervacija { get; set; }
        public int AktivneRezervacije { get; set; }
        public int ZavrseneRezervacije { get; set; }
        public int OtkazaneRezervacije { get; set; }
        public List<RezervacijaIzvjestajRedDto> Stavke { get; set; } = new();
        public DateTime GenerisanoU { get; set; } = DateTime.Now;

        public List<int> RezervacijePoDanima { get; set; } = new();
        public List<KnjigaRankDto> TopKnjige { get; set; } = new();

    }

    // ── US-90: Članovi ──────────────────────────────────────────────────────────

    public class ClanIzvjestajRedDto
    {
        public int RedniBroj { get; set; }
        public string ImePrezime { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string BrojClanske { get; set; } = string.Empty;

        public DateTime DatumRegistracije { get; set; }
        public string StatusClanarine { get; set; } = string.Empty;
        public DateTime? ClanarinaVaziDo { get; set; }
        public int BrojZaduzenja { get; set; }
        public int BrojRezervacija { get; set; }
    }

    public class MjesecniClanoviIzvjestajDto
    {
        public int Mjesec { get; set; }
        public int Godina { get; set; }
        public string NazivMjeseca { get; set; } = string.Empty;
        public int UkupnoAktivnihClanova { get; set; }
        public int NovihClanova { get; set; }
        public int ClanovaAktivnaClanarina { get; set; }
        public int ClanovaIsteklaClanarina { get; set; }
        public List<ClanIzvjestajRedDto> Stavke { get; set; } = new();
        public DateTime GenerisanoU { get; set; } = DateTime.Now;
        public List<int> NoviClanoviPoDanima { get; set; } = new();
        public List<ClanAktivnostDto> TopClanovi { get; set; } = new();
    }

    public class KnjigaRankDto
    {
        public string Naslov { get; set; } = string.Empty;
        public int BrojZaduzenja { get; set; }
    }

    public class ClanAktivnostDto
    {
        public string ImePrezime { get; set; } = string.Empty;
        public int BrojZaduzenja { get; set; }
    }
}