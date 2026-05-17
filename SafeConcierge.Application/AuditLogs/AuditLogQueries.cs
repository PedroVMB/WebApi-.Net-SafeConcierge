using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.AuditLogs.Queries;

public record GetAuditLogsByEntityQuery(string EntityName, Guid EntityId, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<AuditLogDto>>;

public record GetAuditLogsByUserQuery(Guid UserId, DateTime? DateFrom, DateTime? DateTo, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<AuditLogDto>>;

public record GetAuditLogsByCondominiumQuery(Guid CondominiumId, DateTime? DateFrom, DateTime? DateTo, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<AuditLogDto>>;

