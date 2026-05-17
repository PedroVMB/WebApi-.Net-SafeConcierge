using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Enums;

namespace SafeConcierge.Application.Packages.Queries;

public record GetPackageByIdQuery(Guid Id, Guid RequestedByUserId, string RequestedByRole) : IRequest<Result<PackageDetailDto>>;
public record GetPackagesByCondominiumQuery(Guid CondominiumId, PackageStatus? Status, int Page = 1, int PageSize = 20) : IRequest<PagedResult<PackageDto>>;
public record GetMyPackagesQuery(Guid ResidentId, PackageStatus? Status) : IRequest<IEnumerable<PackageDto>>;
public record GetPendingPackagesQuery(Guid CondominiumId) : IRequest<IEnumerable<PackageDto>>;

