using Microsoft.EntityFrameworkCore;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;
using SafeConcierge.Infrastructure.Data;

namespace SafeConcierge.Infrastructure.Repositories;

public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context)
    {
    }

    public IEnumerable<Notification> GetByUserId(Guid userId)
        => DbSet.AsNoTracking()
                .Where(n => n.UserId == userId)
                .ToList();

    public IEnumerable<Notification> GetPendingNotifications()
        => DbSet.AsNoTracking()
                .Where(n => !n.Sent)
                .ToList();
}

