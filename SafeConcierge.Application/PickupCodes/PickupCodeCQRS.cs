using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.PickupCodes.Commands;

public record InvalidatePickupCodeCommand(Guid PackageId, Guid RequestedByUserId) : IRequest<Result<bool>>;
public record RegeneratePickupCodeCommand(Guid PackageId, Guid RequestedByUserId) : IRequest<Result<PickupCodeDto>>;
