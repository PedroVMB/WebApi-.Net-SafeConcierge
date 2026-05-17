using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Application.Condominiums.Queries;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Interfaces;

namespace SafeConcierge.Application.Condominiums.Handlers;

public class GetAllCondominiumsQueryHandler : IRequestHandler<GetAllCondominiumsQuery, IEnumerable<CondominiumDto>>
{
    private readonly IUnitOfWork _uow;
    public GetAllCondominiumsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<IEnumerable<CondominiumDto>> Handle(GetAllCondominiumsQuery request, CancellationToken cancellationToken)
    {
        var result = _uow.Condominiums.GetAll()
            .Select(c => new CondominiumDto(c.Id, c.Name, c.Cnpj, c.Address, c.CreatedAt, c.IsDisabled));
        return Task.FromResult(result);
    }
}

public class GetCondominiumByIdQueryHandler : IRequestHandler<GetCondominiumByIdQuery, Result<CondominiumDto>>
{
    private readonly IUnitOfWork _uow;
    public GetCondominiumByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<Result<CondominiumDto>> Handle(GetCondominiumByIdQuery request, CancellationToken cancellationToken)
    {
        var c = _uow.Condominiums.GetById(request.Id);
        if (c is null) return Task.FromResult(Result<CondominiumDto>.Fail("Condominium not found."));
        return Task.FromResult(Result<CondominiumDto>.Ok(
            new CondominiumDto(c.Id, c.Name, c.Cnpj, c.Address, c.CreatedAt, c.IsDisabled)));
    }
}

