using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Application.DeliveryCompanies.Commands;
using SafeConcierge.Application.DeliveryCompanies.Queries;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;

namespace SafeConcierge.Application.DeliveryCompanies.Handlers;

public class CreateDeliveryCompanyCommandHandler : IRequestHandler<CreateDeliveryCompanyCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CreateDeliveryCompanyCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateDeliveryCompanyCommand request, CancellationToken cancellationToken)
    {
        var existing = _uow.DeliveryCompanies.GetByName(request.Name);
        if (existing is not null) return Result<Guid>.Fail("A delivery company with this name already exists.");

        var company = new DeliveryCompany { Id = Guid.NewGuid(), Name = request.Name, CreatedAt = DateTime.UtcNow };
        _uow.DeliveryCompanies.Add(company);
        await _uow.CommitAsync(cancellationToken);
        return Result<Guid>.Ok(company.Id);
    }
}

public class UpdateDeliveryCompanyCommandHandler : IRequestHandler<UpdateDeliveryCompanyCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public UpdateDeliveryCompanyCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(UpdateDeliveryCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = _uow.DeliveryCompanies.GetById(request.Id);
        if (company is null) return Result<bool>.Fail("Delivery company not found.");

        company.Name = request.Name;
        company.UpdatedAt = DateTime.UtcNow;
        _uow.DeliveryCompanies.Update(company);
        await _uow.CommitAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class DisableDeliveryCompanyCommandHandler : IRequestHandler<DisableDeliveryCompanyCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public DisableDeliveryCompanyCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(DisableDeliveryCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = _uow.DeliveryCompanies.GetById(request.Id);
        if (company is null) return Result<bool>.Fail("Delivery company not found.");

        company.IsDisabled = true;
        company.UpdatedAt = DateTime.UtcNow;
        _uow.DeliveryCompanies.Update(company);
        await _uow.CommitAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class GetAllDeliveryCompaniesQueryHandler : IRequestHandler<GetAllDeliveryCompaniesQuery, IEnumerable<DeliveryCompanyDto>>
{
    private readonly IUnitOfWork _uow;
    public GetAllDeliveryCompaniesQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<IEnumerable<DeliveryCompanyDto>> Handle(GetAllDeliveryCompaniesQuery request, CancellationToken cancellationToken)
    {
        var result = _uow.DeliveryCompanies.GetAll()
            .Select(d => new DeliveryCompanyDto(d.Id, d.Name, d.CreatedAt, d.IsDisabled));
        return Task.FromResult(result);
    }
}

public class GetDeliveryCompanyByIdQueryHandler : IRequestHandler<GetDeliveryCompanyByIdQuery, Result<DeliveryCompanyDto>>
{
    private readonly IUnitOfWork _uow;
    public GetDeliveryCompanyByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<Result<DeliveryCompanyDto>> Handle(GetDeliveryCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var d = _uow.DeliveryCompanies.GetById(request.Id);
        if (d is null) return Task.FromResult(Result<DeliveryCompanyDto>.Fail("Delivery company not found."));
        return Task.FromResult(Result<DeliveryCompanyDto>.Ok(new DeliveryCompanyDto(d.Id, d.Name, d.CreatedAt, d.IsDisabled)));
    }
}

