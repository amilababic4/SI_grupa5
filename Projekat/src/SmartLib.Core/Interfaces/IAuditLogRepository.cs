using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IAuditLogRepository
    {
        // Postojeæe (zadržano)
        Task<IEnumerable<AuditLog>> GetAllAsync(int page, int pageSize);
        Task CreateAsync(AuditLog auditLog);

        // Novo — pretraga i filteri za admin panel
        Task<int> GetTotalCountAsync(string? entitetTip = null, string? akcija = null, int? korisnikId = null);
        Task<IEnumerable<AuditLog>> GetFilteredAsync(
            int page,
            int pageSize,
            string? entitetTip = null,
            string? akcija = null,
            int? korisnikId = null,
            DateTime? odDatuma = null,
            DateTime? doDatuma = null);

        // Historija jednog konkretnog zapisa (npr. sve promjene knjige ID=5)
        Task<IEnumerable<AuditLog>> GetByEntitetAsync(string entitetTip, int entitetId);
    }
}