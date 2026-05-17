using MediatR;
using SafeConcierge.Application.Dashboard.Queries;

namespace SafeConcierge.Api.Endpoints;

public static class DashboardEndpoints
{
    public static RouteGroupBuilder MapDashboardEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/summary", async (Guid condominiumId, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetDashboardSummaryQuery(condominiumId))))
            .RequireAuthorization("AdminOrManager");

        group.MapGet("/packages-by-period", async (Guid condominiumId, DateTime dateFrom, DateTime dateTo, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetPackagesByPeriodQuery(condominiumId, dateFrom, dateTo))))
            .RequireAuthorization("AdminOrManager");

        return group;
    }
}

