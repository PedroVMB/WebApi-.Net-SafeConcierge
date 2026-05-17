using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.Apartments.Commands;

public record CreateApartmentCommand(string Number, int Floor, Guid TowerId, Guid RequestedByUserId) : IRequest<Result<Guid>>;
public record UpdateApartmentCommand(Guid Id, string Number, int Floor, Guid RequestedByUserId) : IRequest<Result<bool>>;
public record DisableApartmentCommand(Guid Id, Guid RequestedByUserId) : IRequest<Result<bool>>;
