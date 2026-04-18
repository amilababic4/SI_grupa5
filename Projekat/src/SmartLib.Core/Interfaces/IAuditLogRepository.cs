using SmartLib.Core.Models;

namespace SmartLib.Core.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<IEnumerable<AuditLog>> GetAllAsync(int page, int pageSize);
        Task CreateAsync(AuditLog auditLog);
    }
}
