using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Application.DeliveryLogs.Commands;
using SafeConcierge.Application.DeliveryLogs.Queries;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Interfaces;

namespace SafeConcierge.Application.DeliveryLogs.Handlers;

public class UploadSignatureCommandHandler : IRequestHandler<UploadSignatureCommand, Result<string>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;

    public UploadSignatureCommandHandler(IUnitOfWork uow, IAuditLogService audit) { _uow = uow; _audit = audit; }

    public async Task<Result<string>> Handle(UploadSignatureCommand request, CancellationToken cancellationToken)
    {
        var log = _uow.DeliveryLogs.GetById(request.DeliveryLogId);
        if (log is null) return Result<string>.Fail("Delivery log not found.");

        // In a real system, upload to blob storage and get URL.
        // For MVP: store as data URL.
        var url = $"data:image/png;base64,{request.SignatureBase64}";
        log.SignatureUrl = url;
        log.UpdatedAt = DateTime.UtcNow;
        _uow.DeliveryLogs.Update(log);
        await _uow.CommitAsync(cancellationToken);

        await _audit.LogAsync(request.RequestedByUserId, "DeliveryLog", request.DeliveryLogId, "UPLOAD_SIGNATURE", cancellationToken: cancellationToken);
        return Result<string>.Ok(url);
    }
}

public class GetDeliveryLogByIdQueryHandler : IRequestHandler<GetDeliveryLogByIdQuery, Result<DeliveryLogDto>>
{
    private readonly IUnitOfWork _uow;
    public GetDeliveryLogByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<Result<DeliveryLogDto>> Handle(GetDeliveryLogByIdQuery request, CancellationToken cancellationToken)
    {
        var d = _uow.DeliveryLogs.GetById(request.Id);
        if (d is null) return Task.FromResult(Result<DeliveryLogDto>.Fail("Delivery log not found."));
        var to = _uow.Users.GetById(d.DeliveredToId);
        var by = _uow.Users.GetById(d.DeliveredById);
        return Task.FromResult(Result<DeliveryLogDto>.Ok(
            new DeliveryLogDto(d.Id, d.PackageId, d.PickupCode, d.DeliveredToId, to?.Name, d.DeliveredById, by?.Name, d.DeliveredAt, d.SignatureUrl)));
    }
}

public class GetDeliveryLogsByPackageQueryHandler : IRequestHandler<GetDeliveryLogsByPackageQuery, IEnumerable<DeliveryLogDto>>
{
    private readonly IUnitOfWork _uow;
    public GetDeliveryLogsByPackageQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<IEnumerable<DeliveryLogDto>> Handle(GetDeliveryLogsByPackageQuery request, CancellationToken cancellationToken)
    {
        var result = _uow.DeliveryLogs.GetByPackageId(request.PackageId)
            .Select(d => new DeliveryLogDto(d.Id, d.PackageId, d.PickupCode, d.DeliveredToId, null, d.DeliveredById, null, d.DeliveredAt, d.SignatureUrl));
        return Task.FromResult(result);
    }
}

public class GetDeliveryLogsByCondominiumQueryHandler : IRequestHandler<GetDeliveryLogsByCondominiumQuery, PagedResult<DeliveryLogDto>>
{
    private readonly IUnitOfWork _uow;
    public GetDeliveryLogsByCondominiumQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<PagedResult<DeliveryLogDto>> Handle(GetDeliveryLogsByCondominiumQuery request, CancellationToken cancellationToken)
    {
        var packages = _uow.Packages.GetByCondominiumId(request.CondominiumId).Select(p => p.Id).ToHashSet();
        var query = _uow.DeliveryLogs.GetAll().Where(d => packages.Contains(d.PackageId));

        if (request.DateFrom.HasValue) query = query.Where(d => d.DeliveredAt >= request.DateFrom.Value);
        if (request.DateTo.HasValue) query = query.Where(d => d.DeliveredAt <= request.DateTo.Value);

        var total = query.Count();
        var items = query.OrderByDescending(d => d.DeliveredAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DeliveryLogDto(d.Id, d.PackageId, d.PickupCode, d.DeliveredToId, null, d.DeliveredById, null, d.DeliveredAt, d.SignatureUrl))
            .ToList();

        return Task.FromResult(new PagedResult<DeliveryLogDto>(items, total, request.Page, request.PageSize));
    }
}

