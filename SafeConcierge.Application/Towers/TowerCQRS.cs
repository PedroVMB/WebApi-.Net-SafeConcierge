using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.Towers.Commands;

public record CreateTowerCommand(string Name, Guid CondominiumId, Guid RequestedByUserId) : IRequest<Result<Guid>>;
public record UpdateTowerCommand(Guid Id, string Name, Guid RequestedByUserId) : IRequest<Result<bool>>;
public record DisableTowerCommand(Guid Id, Guid RequestedByUserId) : IRequest<Result<bool>>;
