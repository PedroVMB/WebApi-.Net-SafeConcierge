using MediatR;
using SafeConcierge.Application.DeliveryLogs.Queries;

namespace SafeConcierge.Api.Endpoints;

public static class PackageDeliveryLogEndpoints
{
    public static RouteGroupBuilder MapPackageDeliveryLogEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/{packageId:guid}/delivery-logs", async (Guid packageId, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetDeliveryLogsByPackageQuery(packageId))))
            .RequireAuthorization();

        return group;
    }
}

