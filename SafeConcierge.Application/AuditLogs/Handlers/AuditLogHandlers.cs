using MediatR;
using SafeConcierge.Application.AuditLogs.Queries;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Interfaces;

namespace SafeConcierge.Application.AuditLogs.Handlers;

public class GetAuditLogsByEntityQueryHandler : IRequestHandler<GetAuditLogsByEntityQuery, PagedResult<AuditLogDto>>
{
    private readonly IUnitOfWork _uow;
    public GetAuditLogsByEntityQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<PagedResult<AuditLogDto>> Handle(GetAuditLogsByEntityQuery request, CancellationToken cancellationToken)
    {
        var query = _uow.AuditLogs.GetByEntityId(request.EntityId)
            .Where(a => a.EntityName == request.EntityName)
            .OrderByDescending(a => a.CreatedAt);

        var total = query.Count();
        var items = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(a => new AuditLogDto(a.Id, a.UserId, null, a.EntityName, a.EntityId, a.Action, a.OldData, a.NewData, a.CreatedAt))
            .ToList();

        return Task.FromResult(new PagedResult<AuditLogDto>(items, total, request.Page, request.PageSize));
    }
}

public class GetAuditLogsByUserQueryHandler : IRequestHandler<GetAuditLogsByUserQuery, PagedResult<AuditLogDto>>
{
    private readonly IUnitOfWork _uow;
    public GetAuditLogsByUserQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<PagedResult<AuditLogDto>> Handle(GetAuditLogsByUserQuery request, CancellationToken cancellationToken)
    {
        var query = _uow.AuditLogs.GetByUserId(request.UserId).AsQueryable();
        if (request.DateFrom.HasValue) query = query.Where(a => a.CreatedAt >= request.DateFrom.Value);
        if (request.DateTo.HasValue) query = query.Where(a => a.CreatedAt <= request.DateTo.Value);
        query = query.OrderByDescending(a => a.CreatedAt);

        var total = query.Count();
        var items = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(a => new AuditLogDto(a.Id, a.UserId, null, a.EntityName, a.EntityId, a.Action, a.OldData, a.NewData, a.CreatedAt))
            .ToList();

        return Task.FromResult(new PagedResult<AuditLogDto>(items, total, request.Page, request.PageSize));
    }
}

public class GetAuditLogsByCondominiumQueryHandler : IRequestHandler<GetAuditLogsByCondominiumQuery, PagedResult<AuditLogDto>>
{
    private readonly IUnitOfWork _uow;
    public GetAuditLogsByCondominiumQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<PagedResult<AuditLogDto>> Handle(GetAuditLogsByCondominiumQuery request, CancellationToken cancellationToken)
    {
        var condominiumUsers = _uow.Users.GetByCondominiumId(request.CondominiumId).Select(u => u.Id).ToHashSet();

        var query = _uow.AuditLogs.GetAll()
            .Where(a => condominiumUsers.Contains(a.UserId))
            .AsQueryable();

        if (request.DateFrom.HasValue) query = query.Where(a => a.CreatedAt >= request.DateFrom.Value);
        if (request.DateTo.HasValue) query = query.Where(a => a.CreatedAt <= request.DateTo.Value);
        query = query.OrderByDescending(a => a.CreatedAt);

        var total = query.Count();
        var items = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(a => new AuditLogDto(a.Id, a.UserId, null, a.EntityName, a.EntityId, a.Action, a.OldData, a.NewData, a.CreatedAt))
            .ToList();

        return Task.FromResult(new PagedResult<AuditLogDto>(items, total, request.Page, request.PageSize));
    }
}

