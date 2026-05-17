using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Application.Notifications.Commands;
using SafeConcierge.Application.Notifications.Queries;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Interfaces;

namespace SafeConcierge.Application.Notifications.Handlers;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public MarkNotificationAsReadCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notif = _uow.Notifications.GetById(request.Id);
        if (notif is null) return Result<bool>.Fail("Notification not found.");
        if (notif.UserId != request.RequestedByUserId) return Result<bool>.Fail("Access denied.");

        notif.Sent = true;
        notif.SentAt = DateTime.UtcNow;
        notif.UpdatedAt = DateTime.UtcNow;
        _uow.Notifications.Update(notif);
        await _uow.CommitAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class ResendNotificationCommandHandler : IRequestHandler<ResendNotificationCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;

    public ResendNotificationCommandHandler(IUnitOfWork uow, IAuditLogService audit) { _uow = uow; _audit = audit; }

    public async Task<Result<bool>> Handle(ResendNotificationCommand request, CancellationToken cancellationToken)
    {
        var notif = _uow.Notifications.GetById(request.Id);
        if (notif is null) return Result<bool>.Fail("Notification not found.");

        // In a real system, trigger push/email send here
        notif.Sent = true;
        notif.SentAt = DateTime.UtcNow;
        notif.UpdatedAt = DateTime.UtcNow;
        _uow.Notifications.Update(notif);
        await _uow.CommitAsync(cancellationToken);

        await _audit.LogAsync(request.RequestedByUserId, "Notification", request.Id, "RESEND", cancellationToken: cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly IUnitOfWork _uow;
    public GetMyNotificationsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<IEnumerable<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var query = _uow.Notifications.GetByUserId(request.UserId).AsQueryable();
        if (request.Sent.HasValue) query = query.Where(n => n.Sent == request.Sent.Value);
        var result = query.OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto(n.Id, n.UserId, n.PackageId, n.Type.ToString(), n.Content, n.Sent, n.SentAt, n.CreatedAt));
        return Task.FromResult<IEnumerable<NotificationDto>>(result.ToList());
    }
}

public class GetNotificationsByPackageQueryHandler : IRequestHandler<GetNotificationsByPackageQuery, IEnumerable<NotificationDto>>
{
    private readonly IUnitOfWork _uow;
    public GetNotificationsByPackageQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<IEnumerable<NotificationDto>> Handle(GetNotificationsByPackageQuery request, CancellationToken cancellationToken)
    {
        var result = _uow.Notifications.GetAll()
            .Where(n => n.PackageId == request.PackageId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto(n.Id, n.UserId, n.PackageId, n.Type.ToString(), n.Content, n.Sent, n.SentAt, n.CreatedAt));
        return Task.FromResult<IEnumerable<NotificationDto>>(result.ToList());
    }
}

