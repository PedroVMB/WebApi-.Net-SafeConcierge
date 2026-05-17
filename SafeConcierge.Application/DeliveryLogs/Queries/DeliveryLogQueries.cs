using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.DeliveryLogs.Queries;

public record GetDeliveryLogByIdQuery(Guid Id) : IRequest<Result<DeliveryLogDto>>;
public record GetDeliveryLogsByPackageQuery(Guid PackageId) : IRequest<IEnumerable<DeliveryLogDto>>;
public record GetDeliveryLogsByCondominiumQuery(Guid CondominiumId, DateTime? DateFrom, DateTime? DateTo, int Page = 1, int PageSize = 20) : IRequest<PagedResult<DeliveryLogDto>>;

