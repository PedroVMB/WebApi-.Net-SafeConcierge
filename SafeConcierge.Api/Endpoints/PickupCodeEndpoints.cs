using MediatR;
using SafeConcierge.Application.PickupCodes.Commands;
using SafeConcierge.Application.PickupCodes.Queries;

namespace SafeConcierge.Api.Endpoints;

public static class PickupCodeEndpoints
{
    public static RouteGroupBuilder MapPickupCodeEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/pickup-code", async (Guid packageId, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var role = GetRole(ctx);
            var result = await mediator.Send(new GetPickupCodeByPackageQuery(packageId, userId, role));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(new { result.Error });
        }).RequireAuthorization();

        group.MapPost("/pickup-code/validate", async (Guid packageId, ValidateCodeRequest req, IMediator mediator) =>
        {
            var result = await mediator.Send(new ValidatePickupCodeQuery(packageId, req.Code));
            return result.IsSuccess ? Results.Ok(new { Valid = true }) : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Doorman");

        group.MapPost("/pickup-code/invalidate", async (Guid packageId, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new InvalidatePickupCodeCommand(packageId, userId));
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("DoormanOrAdmin");

        group.MapPost("/pickup-code/regenerate", async (Guid packageId, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new RegeneratePickupCodeCommand(packageId, userId));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("DoormanOrAdmin");

        return group;
    }

    private static Guid GetUserId(HttpContext ctx) =>
        Guid.Parse(ctx.User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());

    private static string GetRole(HttpContext ctx) =>
        ctx.User.FindFirst("role")?.Value ?? string.Empty;
}

public record ValidateCodeRequest(string Code);

