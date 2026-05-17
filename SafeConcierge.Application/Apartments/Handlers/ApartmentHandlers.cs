using MediatR;
using SafeConcierge.Application.Apartments.Commands;
using SafeConcierge.Application.Apartments.Queries;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;

namespace SafeConcierge.Application.Apartments.Handlers;

public class CreateApartmentCommandHandler : IRequestHandler<CreateApartmentCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;

    public CreateApartmentCommandHandler(IUnitOfWork uow, IAuditLogService audit) { _uow = uow; _audit = audit; }

    public async Task<Result<Guid>> Handle(CreateApartmentCommand request, CancellationToken cancellationToken)
    {
        var tower = _uow.Towers.GetById(request.TowerId);
        if (tower is null) return Result<Guid>.Fail("Tower not found.");

        var dup = _uow.Apartments.GetByTowerId(request.TowerId)
            .Any(a => a.Number.Equals(request.Number, StringComparison.OrdinalIgnoreCase));
        if (dup) return Result<Guid>.Fail("Apartment number already exists in this tower.");

        var apartment = new Apartment { Id = Guid.NewGuid(), Number = request.Number, Floor = request.Floor, TowerId = request.TowerId, CreatedAt = DateTime.UtcNow };
        _uow.Apartments.Add(apartment);
        await _uow.CommitAsync(cancellationToken);
        await _audit.LogAsync(request.RequestedByUserId, nameof(Apartment), apartment.Id, "CREATE", cancellationToken: cancellationToken);
        return Result<Guid>.Ok(apartment.Id);
    }
}

public class UpdateApartmentCommandHandler : IRequestHandler<UpdateApartmentCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;

    public UpdateApartmentCommandHandler(IUnitOfWork uow, IAuditLogService audit) { _uow = uow; _audit = audit; }

    public async Task<Result<bool>> Handle(UpdateApartmentCommand request, CancellationToken cancellationToken)
    {
        var apt = _uow.Apartments.GetById(request.Id);
        if (apt is null) return Result<bool>.Fail("Apartment not found.");

        apt.Number = request.Number;
        apt.Floor = request.Floor;
        apt.UpdatedAt = DateTime.UtcNow;
        _uow.Apartments.Update(apt);
        await _uow.CommitAsync(cancellationToken);
        await _audit.LogAsync(request.RequestedByUserId, nameof(Apartment), request.Id, "UPDATE", cancellationToken: cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class DisableApartmentCommandHandler : IRequestHandler<DisableApartmentCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;

    public DisableApartmentCommandHandler(IUnitOfWork uow, IAuditLogService audit) { _uow = uow; _audit = audit; }

    public async Task<Result<bool>> Handle(DisableApartmentCommand request, CancellationToken cancellationToken)
    {
        var apt = _uow.Apartments.GetById(request.Id);
        if (apt is null) return Result<bool>.Fail("Apartment not found.");

        apt.IsDisabled = true;
        apt.UpdatedAt = DateTime.UtcNow;
        _uow.Apartments.Update(apt);
        await _uow.CommitAsync(cancellationToken);
        await _audit.LogAsync(request.RequestedByUserId, nameof(Apartment), request.Id, "DISABLE", cancellationToken: cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class GetApartmentsByTowerQueryHandler : IRequestHandler<GetApartmentsByTowerQuery, IEnumerable<ApartmentDto>>
{
    private readonly IUnitOfWork _uow;
    public GetApartmentsByTowerQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<IEnumerable<ApartmentDto>> Handle(GetApartmentsByTowerQuery request, CancellationToken cancellationToken)
    {
        var result = _uow.Apartments.GetByTowerId(request.TowerId)
            .Select(a => new ApartmentDto(a.Id, a.Number, a.Floor, a.TowerId, a.CreatedAt, a.IsDisabled));
        return Task.FromResult(result);
    }
}

public class GetApartmentByIdQueryHandler : IRequestHandler<GetApartmentByIdQuery, Result<ApartmentDto>>
{
    private readonly IUnitOfWork _uow;
    public GetApartmentByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<Result<ApartmentDto>> Handle(GetApartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var a = _uow.Apartments.GetById(request.Id);
        if (a is null) return Task.FromResult(Result<ApartmentDto>.Fail("Apartment not found."));
        return Task.FromResult(Result<ApartmentDto>.Ok(new ApartmentDto(a.Id, a.Number, a.Floor, a.TowerId, a.CreatedAt, a.IsDisabled)));
    }
}

