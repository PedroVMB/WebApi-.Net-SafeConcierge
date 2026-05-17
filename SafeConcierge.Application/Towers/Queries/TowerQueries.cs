using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.Towers.Queries;

public record GetTowersByCondominiumQuery(Guid CondominiumId) : IRequest<IEnumerable<TowerDto>>;
public record GetTowerByIdQuery(Guid Id) : IRequest<Result<TowerDto>>;

