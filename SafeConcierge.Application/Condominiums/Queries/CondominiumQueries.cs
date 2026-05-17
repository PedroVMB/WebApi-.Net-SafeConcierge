using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.Condominiums.Queries;

public record GetAllCondominiumsQuery : IRequest<IEnumerable<CondominiumDto>>;
public record GetCondominiumByIdQuery(Guid Id) : IRequest<Result<CondominiumDto>>;

