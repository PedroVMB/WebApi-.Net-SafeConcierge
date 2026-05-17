using Microsoft.EntityFrameworkCore;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;
using SafeConcierge.Infrastructure.Data;

namespace SafeConcierge.Infrastructure.Repositories;

public class AuditLogRepository : RepositoryBase<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(AppDbContext context) : base(context)
    {
    }

    public IEnumerable<AuditLog> GetByUserId(Guid userId)
        => DbSet.AsNoTracking()
                .Where(a => a.UserId == userId)
                .ToList();

    public IEnumerable<AuditLog> GetByEntityId(Guid entityId)
        => DbSet.AsNoTracking()
                .Where(a => a.EntityId == entityId)
                .ToList();
}

