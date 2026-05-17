using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.Notifications.Commands;

public record MarkNotificationAsReadCommand(Guid Id, Guid RequestedByUserId) : IRequest<Result<bool>>;
public record ResendNotificationCommand(Guid Id, Guid RequestedByUserId) : IRequest<Result<bool>>;
