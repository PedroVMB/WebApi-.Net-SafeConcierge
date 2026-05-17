using MediatR;
using SafeConcierge.Application.Common.DTOs;

namespace SafeConcierge.Application.Notifications.Queries;

public record GetMyNotificationsQuery(Guid UserId, bool? Sent = null) : IRequest<IEnumerable<NotificationDto>>;
public record GetNotificationsByPackageQuery(Guid PackageId) : IRequest<IEnumerable<NotificationDto>>;

