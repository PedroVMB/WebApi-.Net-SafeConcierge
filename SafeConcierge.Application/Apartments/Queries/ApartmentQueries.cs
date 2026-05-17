using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.Apartments.Queries;

public record GetApartmentsByTowerQuery(Guid TowerId) : IRequest<IEnumerable<ApartmentDto>>;
public record GetApartmentByIdQuery(Guid Id) : IRequest<Result<ApartmentDto>>;

