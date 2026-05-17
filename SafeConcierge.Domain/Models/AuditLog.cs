namespace SafeConcierge.Domain.Models;

public class AuditLog : Base
{
    public Guid UserId { get; set; }
    public User User { get; set; }

    public string EntityName { get; set; } = string.Empty;

    public Guid EntityId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? OldData { get; set; }

    public string? NewData { get; set; }
}