using SafeConcierge.Domain.Models;

namespace SafeConcierge.Domain.Interfaces;

public interface IAuditLogRepository : IRepositoryBase<AuditLog>
{
    IEnumerable<AuditLog> GetByUserId(Guid userId);
    IEnumerable<AuditLog> GetByEntityId(Guid entityId);
}

