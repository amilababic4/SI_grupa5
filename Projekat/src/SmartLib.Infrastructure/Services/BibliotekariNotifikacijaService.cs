using Microsoft.Extensions.Logging;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.Infrastructure.Services
{
    /// <summary>
    /// Šalje email obavijesti svim aktivnim bibliotekarima i administratorima
    /// pri kreiranju novog zaduženja ili rezervacije.
    /// Injektuje se kao Transient i poziva se direktno iz controllera.
    /// </summary>
    public class BibliotekariNotifikacijaService
    {
        private readonly IKorisnikRepository _korisnikRepo;
        private readonly IEmailService _emailService;
        private readonly ILogger<BibliotekariNotifikacijaService> _logger;

        public BibliotekariNotifikacijaService(
            IKorisnikRepository korisnikRepo,
            IEmailService emailService,
            ILogger<BibliotekariNotifikacijaService> logger)
        {
            _korisnikRepo = korisnikRepo;
            _emailService = emailService;
            _logger = logger;
        }

        // ── Novo zaduženje ─────────────────────────────────────────────────────

        public async Task ObavijestiBibliotekareNovoZaduzenjeAsync(Zaduzenje zaduzenje)
        {
            _logger.LogWarning(">>> DEBUG ObavijestiBibliotekareNovoZaduzenjeAsync: START, ZaduzenjeId={Id}", zaduzenje.Id);

            var korisnik = zaduzenje.Korisnik;
            var knjiga = zaduzenje.Primjerak?.Knjiga;
            var primjerak = zaduzenje.Primjerak;

            _logger.LogWarning(">>> DEBUG: Korisnik={Korisnik}, Knjiga={Knjiga}, Primjerak={Primjerak}",
                korisnik?.Email ?? "NULL",
                knjiga?.Naslov ?? "NULL",
                primjerak?.InventarniBroj ?? "NULL");

            var korisnikIme = korisnik != null ? $"{korisnik.Ime} {korisnik.Prezime}" : $"KorisnikID #{zaduzenje.KorisnikId}";
            var korisnikEmail = korisnik?.Email ?? "-";
            var knjigaNaslov = knjiga?.Naslov ?? "Nepoznata knjiga";
            var knjigaAutor = knjiga?.Autor ?? "";
            var inventarniBroj = primjerak?.InventarniBroj ?? "-";
            var rokVracanja = zaduzenje.DatumPlaniranogVracanja.ToString("dd.MM.yyyy");
            var datumZaduzivanja = zaduzenje.DatumZaduzivanja.ToString("dd.MM.yyyy HH:mm");

            var predmet = $"Novo zaduzenje - {knjigaNaslov} ({korisnikIme})";

            var body = IzgradiHtmlEmail(
                naslovSekcije: "Novo zaduzenje evidentirano",
                ikonaEmoji: "📋",
                badgeHtml: "<span style='background:#2563eb;color:#fff;padding:4px 14px;border-radius:999px;font-size:13px;font-weight:700;'>📋 Novo zaduzenje</span>",
                uvod: "Evidentirano je novo zaduzivanje knjige u sistemu.",
                stavke: new[]
                {
                    ("Clan",             korisnikIme),
                    ("Email clana",      korisnikEmail),
                    ("Knjiga",           knjigaNaslov),
                    ("Autor",            knjigaAutor),
                    ("Inventarni broj",  inventarniBroj),
                    ("Datum zaduzenja",  datumZaduzivanja),
                    ("Rok vracanja",     rokVracanja)
                },
                napomena: "Ova obavijest je automatski generisana. Nema potrebe za akcijom ukoliko je zaduzenje ispravno evidentirano."
            );

            _logger.LogWarning(">>> DEBUG: Email body izgradjen, pozivam PosaljiSvimBibliotekarimaAsync...");
            await PosaljiSvimBibliotekarimaAsync(predmet, body);
            _logger.LogWarning(">>> DEBUG ObavijestiBibliotekareNovoZaduzenjeAsync: END");
        }

        // ── Nova rezervacija ───────────────────────────────────────────────────

        public async Task ObavijestiBibliotekareNovaRezervacijaAsync(Rezervacija rezervacija)
        {
            _logger.LogWarning(">>> DEBUG ObavijestiBibliotekareNovaRezervacijaAsync: START, RezervacijaId={Id}", rezervacija.Id);

            var korisnik = rezervacija.Korisnik;
            var knjiga = rezervacija.Knjiga;

            _logger.LogWarning(">>> DEBUG: Korisnik={Korisnik}, Knjiga={Knjiga}",
                korisnik?.Email ?? "NULL",
                knjiga?.Naslov ?? "NULL");

            var korisnikIme = korisnik != null ? $"{korisnik.Ime} {korisnik.Prezime}" : $"KorisnikID #{rezervacija.KorisnikId}";
            var korisnikEmail = korisnik?.Email ?? "-";
            var knjigaNaslov = knjiga?.Naslov ?? "Nepoznata knjiga";
            var knjigaAutor = knjiga?.Autor ?? "";
            var datumRez = rezervacija.DatumRezervacije.ToString("dd.MM.yyyy HH:mm");
            var datumIsteka = rezervacija.DatumIsteka.HasValue
                                  ? rezervacija.DatumIsteka.Value.ToString("dd.MM.yyyy")
                                  : "-";

            var predmet = $"Nova rezervacija - {knjigaNaslov} ({korisnikIme})";

            var body = IzgradiHtmlEmail(
                naslovSekcije: "Nova rezervacija evidentirana",
                ikonaEmoji: "🔖",
                badgeHtml: "<span style='background:#7c3aed;color:#fff;padding:4px 14px;border-radius:999px;font-size:13px;font-weight:700;'>🔖 Nova rezervacija</span>",
                uvod: "Clan je kreirao novu rezervaciju knjige koja trenutno nije dostupna.",
                stavke: new[]
                {
                    ("Clan",              korisnikIme),
                    ("Email clana",       korisnikEmail),
                    ("Knjiga",            knjigaNaslov),
                    ("Autor",             knjigaAutor),
                    ("Datum rezervacije", datumRez),
                    ("Rezervacija istice", datumIsteka)
                },
                napomena: "Kada knjiga bude vracena, clan ce biti automatski obavijesten. Rezervacija istice za 7 dana ukoliko knjiga ne bude preuzeta."
            );

            _logger.LogWarning(">>> DEBUG: Email body izgradjen, pozivam PosaljiSvimBibliotekarimaAsync...");
            await PosaljiSvimBibliotekarimaAsync(predmet, body);
            _logger.LogWarning(">>> DEBUG ObavijestiBibliotekareNovaRezervacijaAsync: END");
        }

        // ── Privatne pomocne metode ────────────────────────────────────────────

        private async Task PosaljiSvimBibliotekarimaAsync(string predmet, string body)
        {
            _logger.LogWarning(">>> DEBUG PosaljiSvimBibliotekarimaAsync: Dohvatam sve korisnike...");

            var sviKorisnici = (await _korisnikRepo.GetAllAsync()).ToList();

            _logger.LogWarning(">>> DEBUG: Ukupno korisnika u bazi = {Ukupno}", sviKorisnici.Count);

            foreach (var k in sviKorisnici)
            {
                _logger.LogWarning(">>> DEBUG Korisnik: Id={Id}, Email={Email}, Uloga={Uloga}, Status={Status}",
                    k.Id, k.Email, k.Uloga?.Naziv ?? "NULL", k.Status);
            }

            var primaoci = sviKorisnici
                .Where(k => string.Equals(k.Status, "aktivan", StringComparison.OrdinalIgnoreCase) &&
                            (string.Equals(k.Uloga?.Naziv, RoleNames.Bibliotekar, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(k.Uloga?.Naziv, RoleNames.Administrator, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            _logger.LogWarning(">>> DEBUG: Broj primaoca (bibliotekari+admini) = {Broj}", primaoci.Count);

            if (primaoci.Count == 0)
            {
                _logger.LogWarning("[BibliotekariNotifikacija] Nema aktivnih bibliotekara/administratora za slanje emaila.");
                return;
            }

            foreach (var k in primaoci)
            {
                _logger.LogWarning(">>> DEBUG: Saljem email na {Email}...", k.Email);
                try
                {
                    await _emailService.SendEmailAsync(k.Email, predmet, body);
                    _logger.LogWarning(">>> DEBUG: Email USPJESNO poslan na {Email}.", k.Email);
                    _logger.LogInformation("[BibliotekariNotifikacija] Email poslan na {Email}.", k.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ">>> DEBUG: GRESKA pri slanju na {Email}.", k.Email);
                }
            }

            _logger.LogWarning(">>> DEBUG PosaljiSvimBibliotekarimaAsync: ZAVRSENO");
        }

        private static string IzgradiHtmlEmail(
            string naslovSekcije,
            string ikonaEmoji,
            string badgeHtml,
            string uvod,
            IEnumerable<(string Label, string Vrijednost)> stavke,
            string napomena)
        {
            var redovi = string.Concat(stavke
                .Where(s => !string.IsNullOrWhiteSpace(s.Vrijednost) && s.Vrijednost != "-")
                .Select((s, i) => $@"
                    <tr>
                      <td style='font-size:13px;color:#64748b;padding-top:{(i == 0 ? "0" : "8")}px;'>{s.Label}</td>
                      <td style='font-size:13px;color:#1e293b;font-weight:600;text-align:right;padding-top:{(i == 0 ? "0" : "8")}px;'>{s.Vrijednost}</td>
                    </tr>"));

            return $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='margin:0;padding:0;background:#f0f4f8;font-family:Arial,Helvetica,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f0f4f8;padding:40px 0;'>
    <tr><td align='center'>
      <table width='520' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 8px 30px rgba(0,0,0,0.08);'>
        <tr>
          <td style='background:linear-gradient(135deg,#07111f,#173b63);padding:32px 40px;text-align:center;'>
            <h1 style='margin:0;color:#ffffff;font-size:26px;font-weight:800;letter-spacing:-0.5px;'>SmartLib</h1>
            <p style='margin:6px 0 0;color:rgba(255,255,255,0.7);font-size:13px;'>Informacioni sistem za biblioteku</p>
          </td>
        </tr>
        <tr>
          <td style='padding:36px 40px 20px;'>
            <div style='margin-bottom:18px;'>{badgeHtml}</div>
            <h2 style='margin:0 0 16px;color:#1f2937;font-size:20px;font-weight:700;'>{ikonaEmoji} {naslovSekcije}</h2>
            <p style='margin:0 0 24px;color:#374151;font-size:15px;line-height:1.7;'>{uvod}</p>
            <table width='100%' cellpadding='0' cellspacing='0'
                   style='background:#f8fafc;border:1px solid #e2e8f0;border-radius:12px;margin-bottom:24px;'>
              <tr>
                <td style='padding:20px 24px;'>
                  <table width='100%' cellpadding='0' cellspacing='0'>
                    {redovi}
                  </table>
                </td>
              </tr>
            </table>
            <p style='margin:0 0 8px;color:#94a3b8;font-size:13px;line-height:1.5;'>{napomena}</p>
          </td>
        </tr>
        <tr>
          <td style='padding:20px 40px 28px;border-top:1px solid #e2e8f0;'>
            <p style='margin:0;color:#94a3b8;font-size:12px;text-align:center;'>
              © {DateTime.UtcNow.Year} SmartLib · Automatska obavijest sistema
            </p>
          </td>
        </tr>
      </table>
    </td></tr>
  </table>
</body>
</html>";
        }
    }
}