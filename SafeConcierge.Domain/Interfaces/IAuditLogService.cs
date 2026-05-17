namespace SafeConcierge.Domain.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(
        Guid userId,
        string entityName,
        Guid entityId,
        string action,
        string? oldData = null,
        string? newData = null,
        CancellationToken cancellationToken = default);
}

