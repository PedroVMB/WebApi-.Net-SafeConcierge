using MediatR;
using SafeConcierge.Application.Auth.Commands;

namespace SafeConcierge.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/login", async (LoginCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Unauthorized();
        });

        group.MapPut("/change-password", async (ChangePasswordCommand command, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var cmd = command with { UserId = userId };
            var result = await mediator.Send(cmd);
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization();

        return group;
    }

    private static Guid GetUserId(HttpContext ctx) =>
        Guid.Parse(ctx.User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
}

