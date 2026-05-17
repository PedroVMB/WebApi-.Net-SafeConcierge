using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.Dashboard.Queries;

public record GetDashboardSummaryQuery(Guid CondominiumId) : IRequest<DashboardSummaryDto>;

public record PeriodReportDto(string Period, int Count);

public record GetPackagesByPeriodQuery(Guid CondominiumId, DateTime DateFrom, DateTime DateTo)
    : IRequest<IEnumerable<PeriodReportDto>>;

