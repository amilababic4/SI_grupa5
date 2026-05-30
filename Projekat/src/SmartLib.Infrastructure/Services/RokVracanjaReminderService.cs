using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.Infrastructure.Services
{
    /// <summary>
    /// Background service koji se pokreće jednom dnevno i šalje email podsjetnik:
    ///   • 3 dana prije roka  → upozorenje da rok uskoro ističe
    ///   • Na dan roka        → podsjetnik da je danas krajnji rok
    ///   • Nakon isteka roka  → obavijest o kašnjenju (svaki dan sve dok knjiga nije vraćena)
    /// </summary>
    public class RokVracanjaReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RokVracanjaReminderService> _logger;

        // Koliko sati nakon ponoći (UTC) da se pokrene provjera
        private const int PokretanjeNaSat = 23; // 07:00 UTC

        public RokVracanjaReminderService(
            IServiceScopeFactory scopeFactory,
            ILogger<RokVracanjaReminderService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[ReminderService] Pokrenut. Provjera rokova svaki dan u {Sat}:00 UTC.", PokretanjeNaSat);

            while (!stoppingToken.IsCancellationRequested)
            {
                var sada = DateTime.UtcNow;
                var sljedecePokretanje = new DateTime(sada.Year, sada.Month, sada.Day, PokretanjeNaSat, 0, 0, DateTimeKind.Utc);

                // Ako je već prošao sat za danas, planiraj za sutra
                if (sada >= sljedecePokretanje)
                    sljedecePokretanje = sljedecePokretanje.AddDays(1);

                var cekanje = sljedecePokretanje - sada;
                _logger.LogInformation("[ReminderService] Sljedeća provjera za {Cekanje:hh\\:mm\\:ss} (u {Datum}).",
                    cekanje, sljedecePokretanje.ToString("dd.MM.yyyy HH:mm") + " UTC");


                try
                {
                    await Task.Delay(cekanje, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                await ObradiPodsjetnikeAsync(stoppingToken);
            }

            _logger.LogInformation("[ReminderService] Zaustavljen.");
        }

        private async Task ObradiPodsjetnikeAsync(CancellationToken ct)
        {
            _logger.LogInformation("[ReminderService] Pokrenuta obrada podsjetnika...");

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var zaduzenjeRepo = scope.ServiceProvider.GetRequiredService<IZaduzenjeRepository>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var danas = DateTime.UtcNow.Date;

                // ── 1. Zaduženja kojima rok ističe za tačno 3 dana ─────────────────
                var upozorenja = await zaduzenjeRepo.GetAktivnaZaDatumRangeAsync(danas.AddDays(1), danas.AddDays(3));
                foreach (var z in upozorenja)
                {
                    if (z.Korisnik is null) continue;
                    await PosaljiEmailAsync(emailService, z, TipPodsjetnika.Upozorenje3Dana, ct);
                }

                // ── 2. Zaduženja kojima rok ističe danas ───────────────────────────
                var danasnji = await zaduzenjeRepo.GetAktivnaZaDatumAsync(danas);
                foreach (var z in danasnji)
                {
                    if (z.Korisnik is null) continue;
                    await PosaljiEmailAsync(emailService, z, TipPodsjetnika.DanasIstice, ct);
                }

                // ── 3. Kasna zaduženja (rok prošao, knjiga još nije vraćena) ───────
                var kasna = await zaduzenjeRepo.GetKasnaAktivnaAsync(danas);
                foreach (var z in kasna)
                {
                    if (z.Korisnik is null) continue;
                    await PosaljiEmailAsync(emailService, z, TipPodsjetnika.Kasnjenje, ct);
                }

                _logger.LogInformation(
                    "[ReminderService] Obrada završena. Upozorenja: {U}, Danas ističe: {D}, Kašnjenja: {K}.",
                    upozorenja.Count(), danasnji.Count(), kasna.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReminderService] Greška pri obradi podsjetnika.");
            }
        }

        private async Task PosaljiEmailAsync(
            IEmailService emailService,
            Zaduzenje z,
            TipPodsjetnika tip,
            CancellationToken ct)
        {
            var korisnik = z.Korisnik!;
            var knjiga = z.Primjerak?.Knjiga;
            var naslov = knjiga?.Naslov ?? "Nepoznata knjiga";
            var autor = knjiga?.Autor ?? "";
            var rokDatum = z.DatumPlaniranogVracanja.ToString("dd.MM.yyyy");
            var ime = $"{korisnik.Ime} {korisnik.Prezime}";
            var danas = DateTime.UtcNow.Date;
            var danaCekanja = (danas - z.DatumPlaniranogVracanja.Date).Days;

            string predmet;
            string naslovSekcije;
            string porukaUvod;
            string badgeHtml;
            string ikonaEmoji;

            switch (tip)
            {
                case TipPodsjetnika.Upozorenje3Dana:
                    ikonaEmoji = "⏰";
                    predmet = $"Podsjetnik: Rok vraćanja knjige ističe za 3 dana – SmartLib";
                    naslovSekcije = "Rok vraćanja se bliži";
                    porukaUvod = $"Rok za vraćanje knjige <strong>{naslov}</strong> ističe <strong>za 3 dana</strong> ({rokDatum}). Molimo Vas da knjgu pravovremeno vratite.";
                    badgeHtml = "<span style='background:#f59e0b;color:#fff;padding:4px 14px;border-radius:999px;font-size:13px;font-weight:700;'>⏰ Uskoro ističe</span>";
                    break;

                case TipPodsjetnika.DanasIstice:
                    ikonaEmoji = "🔔";
                    predmet = $"Danas je krajnji rok za vraćanje knjige – SmartLib";
                    naslovSekcije = "Danas je krajnji rok vraćanja";
                    porukaUvod = $"Danas (<strong>{rokDatum}</strong>) je krajnji rok za vraćanje knjige <strong>{naslov}</strong>. Molimo Vas da knjgu vratite što je prije moguće.";
                    badgeHtml = "<span style='background:#ef4444;color:#fff;padding:4px 14px;border-radius:999px;font-size:13px;font-weight:700;'>🔔 Danas ističe</span>";
                    break;

                default: // Kasnjenje
                    ikonaEmoji = "⚠️";
                    predmet = $"Upozorenje: Prekoračen rok vraćanja knjige ({danaCekanja} {DanRijec(danaCekanja)}) – SmartLib";
                    naslovSekcije = "Prekoračen rok vraćanja";
                    porukaUvod = $"Rok za vraćanje knjige <strong>{naslov}</strong> je bio <strong>{rokDatum}</strong>. Knjiga kasni <strong>{danaCekanja} {DanRijec(danaCekanja)}</strong>. Molimo Vas da odmah kontaktirate biblioteku ili što hitnije vratite knjigu.";
                    badgeHtml = "<span style='background:#dc2626;color:#fff;padding:4px 14px;border-radius:999px;font-size:13px;font-weight:700;'>⚠️ Prekoračen rok</span>";
                    break;
            }

            var body = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='margin:0;padding:0;background:#f0f4f8;font-family:Arial,Helvetica,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#f0f4f8;padding:40px 0;'>
    <tr><td align='center'>
      <table width='520' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 8px 30px rgba(0,0,0,0.08);'>

        <!-- Header -->
        <tr>
          <td style='background:linear-gradient(135deg,#07111f,#173b63);padding:32px 40px;text-align:center;'>
            <h1 style='margin:0;color:#ffffff;font-size:26px;font-weight:800;letter-spacing:-0.5px;'>📚 SmartLib</h1>
            <p style='margin:6px 0 0;color:rgba(255,255,255,0.7);font-size:13px;'>Informacioni sistem za biblioteku</p>
          </td>
        </tr>

        <!-- Body -->
        <tr>
          <td style='padding:36px 40px 20px;'>

            <!-- Badge -->
            <div style='margin-bottom:18px;'>{badgeHtml}</div>

            <h2 style='margin:0 0 8px;color:#1f2937;font-size:20px;font-weight:700;'>{ikonaEmoji} {naslovSekcije}</h2>
            <p style='margin:0 0 20px;color:#64748b;font-size:15px;'>Poštovani/a <strong>{ime}</strong>,</p>
            <p style='margin:0 0 24px;color:#374151;font-size:15px;line-height:1.7;'>{porukaUvod}</p>

            <!-- Kartica knjige -->
            <table width='100%' cellpadding='0' cellspacing='0'
                   style='background:#f8fafc;border:1px solid #e2e8f0;border-radius:12px;margin-bottom:24px;'>
              <tr>
                <td style='padding:20px 24px;'>
                  <p style='margin:0 0 6px;font-size:13px;color:#94a3b8;text-transform:uppercase;letter-spacing:0.5px;font-weight:600;'>Knjiga</p>
                  <p style='margin:0 0 2px;font-size:17px;font-weight:700;color:#1e293b;'>{naslov}</p>
                  {(string.IsNullOrWhiteSpace(autor) ? "" : $"<p style='margin:0 0 14px;font-size:14px;color:#64748b;'>{autor}</p>")}
                  <hr style='border:none;border-top:1px solid #e2e8f0;margin:12px 0;'>
                  <table width='100%' cellpadding='0' cellspacing='0'>
                    <tr>
                      <td style='font-size:13px;color:#64748b;'>Datum zaduženja</td>
                      <td style='font-size:13px;color:#1e293b;font-weight:600;text-align:right;'>{z.DatumZaduzivanja:dd.MM.yyyy}</td>
                    </tr>
                    <tr>
                      <td style='font-size:13px;color:#64748b;padding-top:6px;'>Rok vraćanja</td>
                      <td style='font-size:13px;font-weight:700;text-align:right;padding-top:6px;
                                 color:{(tip == TipPodsjetnika.Kasnjenje ? "#dc2626" : "#1e293b")};'>{rokDatum}</td>
                    </tr>
                    {(tip == TipPodsjetnika.Kasnjenje ? $@"
                    <tr>
                      <td style='font-size:13px;color:#dc2626;padding-top:6px;font-weight:600;'>Kašnjenje</td>
                      <td style='font-size:13px;color:#dc2626;font-weight:700;text-align:right;padding-top:6px;'>{danaCekanja} {DanRijec(danaCekanja)}</td>
                    </tr>" : "")}
                  </table>
                </td>
              </tr>
            </table>

            <p style='margin:0 0 8px;color:#94a3b8;font-size:13px;line-height:1.5;'>
              Ako ste već vratili knjigu, molimo zanemarite ovu poruku. Za pitanja kontaktirajte biblioteku.
            </p>
          </td>
        </tr>

        <!-- Footer -->
        <tr>
          <td style='padding:20px 40px 28px;border-top:1px solid #e2e8f0;'>
            <p style='margin:0;color:#94a3b8;font-size:12px;text-align:center;'>
              © {DateTime.UtcNow.Year} SmartLib. Sva prava zadržana.
            </p>
          </td>
        </tr>

      </table>
    </td></tr>
  </table>
</body>
</html>";

            try
            {
                await emailService.SendEmailAsync(korisnik.Email, predmet, body);
                _logger.LogInformation(
                    "[ReminderService] Email ({Tip}) poslan korisniku {Email} za zaduženje #{Id}.",
                    tip, korisnik.Email, z.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[ReminderService] Greška pri slanju emaila ({Tip}) korisniku {Email} za zaduženje #{Id}.",
                    tip, korisnik.Email, z.Id);
            }
        }

        // ── Pomoćne metode ─────────────────────────────────────────────────
        private static string DanRijec(int n) => n switch
        {
            1 => "dan",
            _ => "dana"
        };

        private enum TipPodsjetnika
        {
            Upozorenje3Dana,
            DanasIstice,
            Kasnjenje
        }
    }
}