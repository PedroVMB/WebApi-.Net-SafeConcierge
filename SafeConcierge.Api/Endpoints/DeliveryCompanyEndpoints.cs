using MediatR;
using SafeConcierge.Application.DeliveryCompanies.Commands;
using SafeConcierge.Application.DeliveryCompanies.Queries;

namespace SafeConcierge.Api.Endpoints;

public static class DeliveryCompanyEndpoints
{
    public static RouteGroupBuilder MapDeliveryCompanyEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetAllDeliveryCompaniesQuery())))
            .RequireAuthorization("DoormanOrAdmin");

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetDeliveryCompanyByIdQuery(id));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { result.Error });
        }).RequireAuthorization("DoormanOrAdmin");

        group.MapPost("/", async (CreateDeliveryCompanyRequest req, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new CreateDeliveryCompanyCommand(req.Name, userId));
            return result.IsSuccess ? Results.Created($"/api/v1/delivery-companies/{result.Value}", result.Value) : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        group.MapPut("/{id:guid}", async (Guid id, UpdateDeliveryCompanyRequest req, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new UpdateDeliveryCompanyCommand(id, req.Name, userId));
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new DisableDeliveryCompanyCommand(id, userId));
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        return group;
    }

    private static Guid GetUserId(HttpContext ctx) =>
        Guid.Parse(ctx.User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
}

public record CreateDeliveryCompanyRequest(string Name);
public record UpdateDeliveryCompanyRequest(string Name);

