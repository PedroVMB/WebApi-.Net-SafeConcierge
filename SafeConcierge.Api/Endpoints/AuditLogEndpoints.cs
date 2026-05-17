using MediatR;
using SafeConcierge.Application.AuditLogs.Queries;

namespace SafeConcierge.Api.Endpoints;

public static class AuditLogEndpoints
{
    public static RouteGroupBuilder MapAuditLogEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/by-entity", async (string entityName, Guid entityId, int page, int pageSize, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetAuditLogsByEntityQuery(entityName, entityId, page < 1 ? 1 : page, pageSize < 1 ? 20 : pageSize))))
            .RequireAuthorization("Admin");

        group.MapGet("/by-user/{userId:guid}", async (Guid userId, DateTime? dateFrom, DateTime? dateTo, int page, int pageSize, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetAuditLogsByUserQuery(userId, dateFrom, dateTo, page < 1 ? 1 : page, pageSize < 1 ? 20 : pageSize))))
            .RequireAuthorization("Admin");

        group.MapGet("/by-condominium/{condominiumId:guid}", async (Guid condominiumId, DateTime? dateFrom, DateTime? dateTo, int page, int pageSize, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetAuditLogsByCondominiumQuery(condominiumId, dateFrom, dateTo, page < 1 ? 1 : page, pageSize < 1 ? 20 : pageSize))))
            .RequireAuthorization("AdminOrManager");

        return group;
    }
}

