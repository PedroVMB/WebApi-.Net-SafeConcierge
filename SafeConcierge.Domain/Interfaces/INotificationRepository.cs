using SafeConcierge.Domain.Models;

namespace SafeConcierge.Domain.Interfaces;

public interface INotificationRepository : IRepositoryBase<Notification>
{
    IEnumerable<Notification> GetByUserId(Guid userId);
    IEnumerable<Notification> GetPendingNotifications();
}

