using MediatR;
using SafeConcierge.Application.DeliveryLogs.Commands;
using SafeConcierge.Application.DeliveryLogs.Queries;

namespace SafeConcierge.Api.Endpoints;

public static class DeliveryLogEndpoints
{
    public static RouteGroupBuilder MapDeliveryLogEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (Guid condominiumId, DateTime? dateFrom, DateTime? dateTo, int page, int pageSize, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetDeliveryLogsByCondominiumQuery(condominiumId, dateFrom, dateTo, page < 1 ? 1 : page, pageSize < 1 ? 20 : pageSize))))
            .RequireAuthorization("AdminOrManager");

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetDeliveryLogByIdQuery(id));
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(new { result.Error });
        }).RequireAuthorization("Staff");

        group.MapPost("/{id:guid}/signature", async (Guid id, UploadSignatureRequest req, IMediator mediator, HttpContext ctx) =>
        {
            var userId = GetUserId(ctx);
            var result = await mediator.Send(new UploadSignatureCommand(id, req.SignatureBase64, userId));
            return result.IsSuccess ? Results.Ok(new { Url = result.Value }) : Results.BadRequest(new { result.Error });
        }).RequireAuthorization("Doorman");

        return group;
    }

    private static Guid GetUserId(HttpContext ctx) =>
        Guid.Parse(ctx.User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
}

public record UploadSignatureRequest(string SignatureBase64);

