using MediatR;
using SafeConcierge.Application.Notifications.Commands;
using SafeConcierge.Application.Notifications.Queries;

namespace SafeConcierge.Api.Endpoints;

public static class NotificationEndpoints
{
    public static RouteGroupBuilder MapNotificationEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/mine", async (bool? sent, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            return Results.Ok(await mediator.Send(new GetMyNotificationsQuery(userId, sent)));
        }).RequireAuthorization();

        group.MapGet("/by-package/{packageId:guid}", async (Guid packageId, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetNotificationsByPackageQuery(packageId))))
            .RequireAuthorization("Staff");

        group.MapPut("/{id:guid}/read", async (Guid id, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new MarkNotificationAsReadCommand(id, userId));
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization();

        group.MapPost("/{id:guid}/resend", async (Guid id, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new ResendNotificationCommand(id, userId));
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("AdminOrManager");

        return group;
    }

    private static Guid GetUserId(HttpContext ctx) =>
        Guid.Parse(ctx.User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
}

