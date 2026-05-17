using MediatR;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.Condominiums.Commands;

public record CreateCondominiumCommand(string Name, string Cnpj, string Address) : IRequest<Result<Guid>>;
public record UpdateCondominiumCommand(Guid Id, string Name, string Cnpj, string Address) : IRequest<Result<bool>>;
public record DisableCondominiumCommand(Guid Id, Guid RequestedByUserId) : IRequest<Result<bool>>;

