using MediatR;
using SafeConcierge.Application.Condominiums.Commands;
using SafeConcierge.Application.Condominiums.Queries;

namespace SafeConcierge.Api.Endpoints;

public static class CondominiumEndpoints
{
    public static RouteGroupBuilder MapCondominiumEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetAllCondominiumsQuery())))
            .RequireAuthorization("Admin");

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCondominiumByIdQuery(id));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { result.Error });
        }).RequireAuthorization("AdminOrManager");

        group.MapPost("/", async (CreateCondominiumCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result.IsSuccess ? Results.Created($"/api/v1/condominiums/{result.Value}", result.Value) : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        group.MapPut("/{id:guid}", async (Guid id, UpdateCondominiumCommand command, IMediator mediator) =>
        {
            var cmd = command with { Id = id };
            var result = await mediator.Send(cmd);
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new DisableCondominiumCommand(id, userId));
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        return group;
    }

    private static Guid GetUserId(HttpContext ctx) =>
        Guid.Parse(ctx.User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
}

