using SafeConcierge.Domain.Enums;

namespace SafeConcierge.Domain.Models;

public class Notification : Base
{
    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid? PackageId { get; set; }
    public Package? Package { get; set; }

    public NotificationType Type { get; set; }

    public string Content { get; set; } = string.Empty;

    public bool Sent { get; set; }

    public DateTime? SentAt { get; set; }
}