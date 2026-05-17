using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Application.Dashboard.Queries;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Enums;
using SafeConcierge.Domain.Interfaces;

namespace SafeConcierge.Application.Dashboard.Handlers;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IUnitOfWork _uow;
    public GetDashboardSummaryQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var packages = _uow.Packages.GetByCondominiumId(request.CondominiumId).ToList();
        var total = packages.Count;
        var pending = packages.Count(p => p.Status == PackageStatus.WAITING_PICKUP);
        var delivered = packages.Count(p => p.Status == PackageStatus.DELIVERED);
        var expired = packages.Count(p => p.Status == PackageStatus.EXPIRED);
        var canceled = packages.Count(p => p.Status == PackageStatus.CANCELED);

        var deliveredPackages = packages.Where(p => p.Status == PackageStatus.DELIVERED && p.DeliveredAt.HasValue).ToList();
        var avgPickup = deliveredPackages.Any()
            ? deliveredPackages.Average(p => (p.DeliveredAt!.Value - p.ReceivedAt).TotalHours)
            : 0;

        return Task.FromResult(new DashboardSummaryDto(total, pending, delivered, expired, canceled, Math.Round(avgPickup, 2)));
    }
}

public class GetPackagesByPeriodQueryHandler : IRequestHandler<GetPackagesByPeriodQuery, IEnumerable<PeriodReportDto>>
{
    private readonly IUnitOfWork _uow;
    public GetPackagesByPeriodQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<IEnumerable<PeriodReportDto>> Handle(GetPackagesByPeriodQuery request, CancellationToken cancellationToken)
    {
        var packages = _uow.Packages.GetByCondominiumId(request.CondominiumId)
            .Where(p => p.ReceivedAt >= request.DateFrom && p.ReceivedAt <= request.DateTo)
            .ToList();

        var grouped = packages
            .GroupBy(p => p.ReceivedAt.ToString("yyyy-MM-dd"))
            .Select(g => new PeriodReportDto(g.Key, g.Count()))
            .OrderBy(x => x.Period);

        return Task.FromResult<IEnumerable<PeriodReportDto>>(grouped.ToList());
    }
}

