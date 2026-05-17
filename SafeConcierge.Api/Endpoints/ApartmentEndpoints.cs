using MediatR;
using SafeConcierge.Application.Apartments.Commands;
using SafeConcierge.Application.Apartments.Queries;

namespace SafeConcierge.Api.Endpoints;

public static class ApartmentEndpoints
{
    public static RouteGroupBuilder MapApartmentEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (Guid towerId, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetApartmentsByTowerQuery(towerId))))
            .RequireAuthorization("Staff");

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetApartmentByIdQuery(id));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { result.Error });
        }).RequireAuthorization("Staff");

        group.MapPost("/", async (Guid towerId, CreateApartmentRequest req, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new CreateApartmentCommand(req.Number, req.Floor, towerId, userId));
            return result.IsSuccess ? Results.Created($"/api/v1/apartments/{result.Value}", result.Value) : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        group.MapPut("/{id:guid}", async (Guid id, UpdateApartmentRequest req, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new UpdateApartmentCommand(id, req.Number, req.Floor, userId));
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new DisableApartmentCommand(id, userId));
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Admin");

        return group;
    }

    private static Guid GetUserId(HttpContext ctx) =>
        Guid.Parse(ctx.User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
}

public record CreateApartmentRequest(string Number, int Floor);
public record UpdateApartmentRequest(string Number, int Floor);

