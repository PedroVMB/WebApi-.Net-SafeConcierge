using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;

namespace SafeConcierge.Application.DeliveryCompanies.Queries;

public record GetAllDeliveryCompaniesQuery : IRequest<IEnumerable<DeliveryCompanyDto>>;
public record GetDeliveryCompanyByIdQuery(Guid Id) : IRequest<Result<DeliveryCompanyDto>>;

