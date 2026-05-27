using System.Text.Json;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;

namespace SmartLib.Infrastructure.Services
{
    /// <summary>
    /// Centralni servis za pisanje audit logova.
    /// Injectuje se u repositorije (KorisnikRepository, KnjigaRepository, itd.)
    /// i poziva se nakon svake CREATE / UPDATE / DELETE operacije.
    /// </summary>
    public class AuditLogService
    {
        private readonly IAuditLogRepository _auditRepo;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public AuditLogService(IAuditLogRepository auditRepo)
        {
            _auditRepo = auditRepo;
        }

        // ── Javni API ─────────────────────────────────────────────────────────

        // ZAMIJENI cijelu WriteAsync metodu i javni API:
        public Task LogCreateAsync(object entitet, string entitetTip, int entitetId, int? korisnikId = null)
            => WriteAsync(entitetTip, entitetId, "CREATE", null, entitet, korisnikId);

        public Task LogUpdateAsync(object staro, object novo, string entitetTip, int entitetId, int? korisnikId = null)
            => WriteAsync(entitetTip, entitetId, "UPDATE", staro, novo, korisnikId);

        public Task LogDeleteAsync(object entitet, string entitetTip, int entitetId, int? korisnikId = null)
            => WriteAsync(entitetTip, entitetId, "DELETE", entitet, null, korisnikId);

        private async Task WriteAsync(
            string entitetTip, int entitetId, string akcija,
            object? prije, object? nakon, int? korisnikId)
        {
            var log = new AuditLog
            {
                EntitetTip = entitetTip,
                EntitetId = entitetId,
                Akcija = akcija,
                KorisnikId = korisnikId,
                DatumAkcije = DateTime.UtcNow,
                VrijednostiPrije = prije is null ? null : Serijalizuj(prije),
                VrijednostiNakon = nakon is null ? null : Serijalizuj(nakon)
            };
            await _auditRepo.CreateAsync(log);
        }

        private static string Serijalizuj<T>(T obj)
        {
            try { return JsonSerializer.Serialize(obj, _jsonOpts); }
            catch { return "{}"; }
        }
    }
}