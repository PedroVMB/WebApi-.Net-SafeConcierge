using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.PickupCodes.Queries;

public record GetPickupCodeByPackageQuery(Guid PackageId, Guid RequestedByUserId, string RequestedByRole) : IRequest<Result<PickupCodeDto>>;
public record ValidatePickupCodeQuery(Guid PackageId, string Code) : IRequest<Result<bool>>;

