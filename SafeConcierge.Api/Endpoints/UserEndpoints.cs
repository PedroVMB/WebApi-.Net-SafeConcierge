using MediatR;
using SafeConcierge.Application.Users.Commands;
using SafeConcierge.Application.Users.Queries;
using SafeConcierge.Domain.Enums;

namespace SafeConcierge.Api.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (Guid condominiumId, UserRole? role, bool? active, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetUsersByCondominiumQuery(condominiumId, role, active))))
            .RequireAuthorization("AdminOrManager");

        group.MapGet("/me", async (IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new GetCurrentUserQuery(userId));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound();
        }).RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUserByIdQuery(id));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { result.Error });
        }).RequireAuthorization("AdminOrManager");

        group.MapPost("/", async (CreateUserCommand command, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var cmd = command with { RequestedByUserId = userId };
            var result = await mediator.Send(cmd);
            return result.IsSuccess ? Results.Created($"/api/v1/users/{result.Value}", result.Value) : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        group.MapPut("/{id:guid}", async (Guid id, UpdateUserCommand command, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var cmd = command with { Id = id, RequestedByUserId = userId };
            var result = await mediator.Send(cmd);
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new DeactivateUserCommand(id, userId));
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        group.MapPut("/{id:guid}/reactivate", async (Guid id, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new ReactivateUserCommand(id, userId));
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        return group;
    }

    private static Guid GetUserId(HttpContext ctx) =>
        Guid.Parse(ctx.User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
}

