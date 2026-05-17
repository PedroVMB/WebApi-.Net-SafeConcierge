using MediatR;
using SafeConcierge.Application.Packages.Commands;
using SafeConcierge.Application.Packages.Queries;
using SafeConcierge.Domain.Enums;

namespace SafeConcierge.Api.Endpoints;

public static class PackageEndpoints
{
    public static RouteGroupBuilder MapPackageEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (Guid condominiumId, PackageStatus? status, int page, int pageSize, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetPackagesByCondominiumQuery(condominiumId, status, page < 1 ? 1 : page, pageSize < 1 ? 20 : pageSize))))
            .RequireAuthorization("Staff");

        group.MapGet("/pending", async (Guid condominiumId, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetPendingPackagesQuery(condominiumId))))
            .RequireAuthorization("Staff");

        group.MapGet("/mine", async (PackageStatus? status, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            return Results.Ok(await mediator.Send(new GetMyPackagesQuery(userId, status)));
        }).RequireAuthorization("Resident");

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var role = GetRole(ctx);
            var result = await mediator.Send(new GetPackageByIdQuery(id, userId, role));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { result.Error });
        }).RequireAuthorization();

        group.MapPost("/", async (RegisterPackageCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result.IsSuccess ? Results.Created($"/api/v1/packages/{result.Value}", result.Value) : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Doorman");

        group.MapPut("/{id:guid}", async (Guid id, UpdatePackageRequest req, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new UpdatePackageCommand(id, req.Description, req.DeliveryCompanyId, userId));
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("DoormanOrAdmin");

        group.MapPost("/{id:guid}/pickup", async (Guid id, PickupPackageRequest req, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new PickupPackageCommand(id, req.Code, req.ResidentId, userId));
            return result.IsSuccess ? Results.Ok(new { DeliveryLogId = result.Value }) : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Doorman");

        group.MapPost("/{id:guid}/cancel", async (Guid id, CancelPackageRequest req, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new CancelPackageCommand(id, req.Reason, userId));
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("DoormanOrAdmin");

        group.MapPost("/{id:guid}/reminder", async (Guid id, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new SendPackageReminderCommand(id, userId));
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Staff");

        return group;
    }

    private static Guid GetUserId(HttpContext ctx) =>
        Guid.Parse(ctx.User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());

    private static string GetRole(HttpContext ctx) =>
        ctx.User.FindFirst("role")?.Value ?? string.Empty;
}

public record UpdatePackageRequest(string Description, Guid? DeliveryCompanyId);
public record PickupPackageRequest(string Code, Guid ResidentId);
public record CancelPackageRequest(string Reason);

