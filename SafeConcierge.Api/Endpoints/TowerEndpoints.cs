using MediatR;
using SafeConcierge.Application.Towers.Commands;
using SafeConcierge.Application.Towers.Queries;

namespace SafeConcierge.Api.Endpoints;

public static class TowerEndpoints
{
    public static RouteGroupBuilder MapTowerEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (Guid condominiumId, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetTowersByCondominiumQuery(condominiumId))))
            .RequireAuthorization("Staff");

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetTowerByIdQuery(id));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { result.Error });
        }).RequireAuthorization("Staff");

        group.MapPost("/", async (Guid condominiumId, CreateTowerRequest req, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new CreateTowerCommand(req.Name, condominiumId, userId));
            return result.IsSuccess ? Results.Created($"/api/v1/towers/{result.Value}", result.Value) : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        group.MapPut("/{id:guid}", async (Guid id, UpdateTowerRequest req, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new UpdateTowerCommand(id, req.Name, userId));
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new DisableTowerCommand(id, userId));
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        return group;
    }

    private static Guid GetUserId(HttpContext ctx) =>
        Guid.Parse(ctx.User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
}

public record CreateTowerRequest(string Name);
public record UpdateTowerRequest(string Name);

