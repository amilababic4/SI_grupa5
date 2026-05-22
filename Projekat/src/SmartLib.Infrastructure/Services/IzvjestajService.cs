using Microsoft.EntityFrameworkCore;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Data;
using System.Globalization;

namespace SmartLib.Infrastructure.Services
{
    public class IzvjestajService : IIzvjestajService
    {
        private readonly ApplicationDbContext _db;
        private static readonly TimeZoneInfo LokalnaVremenskaZona = ResolveLokalnaVremenskaZona();

        private static readonly string[] NaziviMjeseci =
        [
            "Januar", "Februar", "Mart", "April", "Maj", "Juni",
            "Juli", "August", "Septembar", "Oktobar", "Novembar", "Decembar"
        ];

        public IzvjestajService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<MjesecniZaduzenjaIzvjestajDto> GenerirajMjesecnaZaduzenja(int mjesec, int godina)
        {
            var pocetak = new DateTime(godina, mjesec, 1, 0, 0, 0, DateTimeKind.Utc);
            var kraj = pocetak.AddMonths(1);

            var zaduzenja = await _db.Zaduzenja
                .AsNoTracking()
                .Include(z => z.Korisnik)
                .Include(z => z.Primjerak)
                    .ThenInclude(p => p!.Knjiga)
                .Where(z => z.DatumZaduzivanja >= pocetak && z.DatumZaduzivanja < kraj)
                .OrderBy(z => z.DatumZaduzivanja)
                .ToListAsync();

            var stavke = zaduzenja.Select((z, i) => new ZaduzenjeIzvjestajRedDto
            {
                RedniBroj = i + 1,
                ClanImePrezime = $"{z.Korisnik?.Ime} {z.Korisnik?.Prezime}".Trim(),
                ClanEmail = z.Korisnik?.Email ?? "-",
                NaslovKnjige = z.Primjerak?.Knjiga?.Naslov ?? "-",
                Autor = z.Primjerak?.Knjiga?.Autor ?? "-",
                InventarniBroj = z.Primjerak?.InventarniBroj ?? "-",
                DatumZaduzivanja = z.DatumZaduzivanja,
                DatumPlaniranogVracanja = z.DatumPlaniranogVracanja,
                DatumStvarnogVracanja = z.DatumStvarnogVracanja,
                Status = z.Status
            }).ToList();

            int danUMjesecu = DateTime.DaysInMonth(godina, mjesec);
            var zaduzenjaPoDanima = Enumerable.Range(1, danUMjesecu)
                .Select(d => zaduzenja.Count(z => z.DatumZaduzivanja.Day == d))
                .ToList();

            var topKnjige = zaduzenja
                .GroupBy(z => z.Primjerak?.Knjiga?.Naslov ?? "-")
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new KnjigaRankDto { Naslov = g.Key, BrojZaduzenja = g.Count() })
                .ToList();
            return new MjesecniZaduzenjaIzvjestajDto
            {
                Mjesec = mjesec,
                Godina = godina,
                NazivMjeseca = NaziviMjeseci[mjesec - 1],
                UkupnoZaduzenja = zaduzenja.Count,
                AktivnaZaduzenja = zaduzenja.Count(z => z.Status == "aktivno"),
                ZatvorenaZaduzenja = zaduzenja.Count(z => z.Status == "zatvoreno"),
                ZakasnjelaZaduzenja = zaduzenja.Count(z => z.Status == "zakašnjelo"),
                Stavke = stavke,
                ZaduzenjaPoDanima = zaduzenjaPoDanima,
                TopKnjige = topKnjige,
                GenerisanoU = GetLokalnoVrijeme()
            };
        }

        public async Task<MjesecneRezervacijeIzvjestajDto> GenerirajMjesecneRezervacije(int mjesec, int godina)
        {
            var pocetak = new DateTime(godina, mjesec, 1, 0, 0, 0, DateTimeKind.Utc);
            var kraj = pocetak.AddMonths(1);

            var rezervacije = await _db.Rezervacije
                .AsNoTracking()
                .Include(r => r.Korisnik)
                .Include(r => r.Knjiga)
                .Where(r => r.DatumRezervacije >= pocetak && r.DatumRezervacije < kraj)
                .OrderBy(r => r.DatumRezervacije)
                .ToListAsync();

            var stavke = rezervacije.Select((r, i) => new RezervacijaIzvjestajRedDto
            {
                RedniBroj = i + 1,
                ClanImePrezime = $"{r.Korisnik?.Ime} {r.Korisnik?.Prezime}".Trim(),
                ClanEmail = r.Korisnik?.Email ?? "-",
                NaslovKnjige = r.Knjiga?.Naslov ?? "-",
                Autor = r.Knjiga?.Autor ?? "-",
                DatumRezervacije = r.DatumRezervacije,
                DatumIsteka = r.DatumIsteka,
                Status = r.Status
            }).ToList();

            int danUMjesecu = DateTime.DaysInMonth(godina, mjesec);
            var rezervacijePoDanima = Enumerable.Range(1, danUMjesecu)
                .Select(d => rezervacije.Count(r => r.DatumRezervacije.Day == d))
                .ToList();

            var topKnjige = rezervacije
                .GroupBy(r => r.Knjiga?.Naslov ?? "-")
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new KnjigaRankDto { Naslov = g.Key, BrojZaduzenja = g.Count() })
                .ToList();
            return new MjesecneRezervacijeIzvjestajDto
            {
                Mjesec = mjesec,
                Godina = godina,
                NazivMjeseca = NaziviMjeseci[mjesec - 1],
                UkupnoRezervacija = rezervacije.Count,
                AktivneRezervacije = rezervacije.Count(r => r.Status == "aktivna"),
                ZavrseneRezervacije = rezervacije.Count(r => r.Status == "završena"),
                OtkazaneRezervacije = rezervacije.Count(r => r.Status == "otkazana"),
                Stavke = stavke,
                RezervacijePoDanima = rezervacijePoDanima,
                TopKnjige = topKnjige,
                GenerisanoU = GetLokalnoVrijeme()
            };
        }

        public async Task<MjesecniClanoviIzvjestajDto> GenerirajMjesecneKlanove(int mjesec, int godina)
        {
            var pocetak = new DateTime(godina, mjesec, 1, 0, 0, 0, DateTimeKind.Utc);
            var kraj = pocetak.AddMonths(1);
            var danas = DateTime.UtcNow;

            // Svi aktivni korisnici (uloga Clan) koji su registrovani do kraja odabranog perioda
            var korisnici = await _db.Korisnici
                .AsNoTracking()
                .Include(k => k.Clanarine)
                .Include(k => k.Zaduzenja)
                .Include(k => k.Rezervacije)
                .Where(k => k.Uloga != null && k.Uloga.Naziv == RoleNames.Clan && k.DatumKreiranja < kraj)
                .OrderBy(k => k.DatumKreiranja)
                .ToListAsync();

            var stavke = korisnici.Select((k, i) =>
            {
                var aktivnaClanarina = k.Clanarine?
                    .FirstOrDefault(c => c.DatumPocetka <= danas && c.DatumIsteka >= danas);

                return new ClanIzvjestajRedDto
                {
                    RedniBroj = i + 1,
                    ImePrezime = $"{k.Ime} {k.Prezime}".Trim(),
                    Email = k.Email ?? "-",
                    BrojClanske = "-",
                    DatumRegistracije = k.DatumKreiranja,
                    StatusClanarine = aktivnaClanarina != null ? "Aktivna" : "Istekla",
                    ClanarinaVaziDo = aktivnaClanarina?.DatumIsteka,
                    BrojZaduzenja = k.Zaduzenja?.Count(z =>
                        z.DatumZaduzivanja >= pocetak && z.DatumZaduzivanja < kraj) ?? 0,
                    BrojRezervacija = k.Rezervacije?.Count(r =>
                        r.DatumRezervacije >= pocetak && r.DatumRezervacije < kraj) ?? 0
                };
            }).ToList();

            int danUMjesecu = DateTime.DaysInMonth(godina, mjesec);
            var noviClanoviPoDanima = Enumerable.Range(1, danUMjesecu)
                .Select(d => korisnici.Count(k =>
                    k.DatumKreiranja >= pocetak &&
                    k.DatumKreiranja < kraj &&
                    k.DatumKreiranja.Day == d))
                .ToList();

            var topClanovi = korisnici
                .Select(k => new ClanAktivnostDto
                {
                    ImePrezime = $"{k.Ime} {k.Prezime}".Trim(),
                    BrojZaduzenja = k.Zaduzenja?.Count(z =>
                        z.DatumZaduzivanja >= pocetak &&
                        z.DatumZaduzivanja < kraj) ?? 0
                })
                .Where(c => c.BrojZaduzenja > 0)
                .OrderByDescending(c => c.BrojZaduzenja)
                .Take(5)
                .ToList();
            return new MjesecniClanoviIzvjestajDto
            {
                Mjesec = mjesec,
                Godina = godina,
                NazivMjeseca = NaziviMjeseci[mjesec - 1],
                UkupnoAktivnihClanova = korisnici.Count,
                NovihClanova = korisnici.Count(k =>
                    k.DatumKreiranja >= pocetak && k.DatumKreiranja < kraj),
                ClanovaAktivnaClanarina = stavke.Count(s => s.StatusClanarine == "Aktivna"),
                ClanovaIsteklaClanarina = stavke.Count(s => s.StatusClanarine == "Istekla"),
                Stavke = stavke,
                NoviClanoviPoDanima = noviClanoviPoDanima,
                TopClanovi = topClanovi,
                GenerisanoU = GetLokalnoVrijeme()
            };
        }

        private static DateTime GetLokalnoVrijeme()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, LokalnaVremenskaZona);
        }

        private static TimeZoneInfo ResolveLokalnaVremenskaZona()
        {
            string[] candidates = ["Europe/Sarajevo", "Central European Standard Time"];

            foreach (var id in candidates)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch (TimeZoneNotFoundException)
                {
                }
                catch (InvalidTimeZoneException)
                {
                }
            }

            return TimeZoneInfo.Local;
        }
    }
}