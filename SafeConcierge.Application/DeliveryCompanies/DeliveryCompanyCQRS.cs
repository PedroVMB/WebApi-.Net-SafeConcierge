using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.DeliveryCompanies.Commands;

public record CreateDeliveryCompanyCommand(string Name, Guid RequestedByUserId) : IRequest<Result<Guid>>;
public record UpdateDeliveryCompanyCommand(Guid Id, string Name, Guid RequestedByUserId) : IRequest<Result<bool>>;
public record DisableDeliveryCompanyCommand(Guid Id, Guid RequestedByUserId) : IRequest<Result<bool>>;
