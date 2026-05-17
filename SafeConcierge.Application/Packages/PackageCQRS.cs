using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Enums;

namespace SafeConcierge.Application.Packages.Commands;

public record RegisterPackageCommand(
    Guid CondominiumId, Guid ApartmentId, Guid ResidentId,
    Guid? DeliveryCompanyId, Guid ReceivedById,
    string Description, DateTime ReceivedAt) : IRequest<Result<Guid>>;

public record UpdatePackageCommand(
    Guid Id, string Description, Guid? DeliveryCompanyId,
    Guid RequestedByUserId) : IRequest<Result<bool>>;

public record CancelPackageCommand(Guid Id, string Reason, Guid RequestedByUserId) : IRequest<Result<bool>>;

public record PickupPackageCommand(
    Guid PackageId, string Code, Guid ResidentId,
    Guid DeliveredById) : IRequest<Result<Guid>>;

public record SendPackageReminderCommand(Guid PackageId, Guid RequestedByUserId) : IRequest<Result<bool>>;
