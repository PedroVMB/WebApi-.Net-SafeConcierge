using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Application.Towers.Commands;
using SafeConcierge.Application.Towers.Queries;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;

namespace SafeConcierge.Application.Towers.Handlers;

public class CreateTowerCommandHandler : IRequestHandler<CreateTowerCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;

    public CreateTowerCommandHandler(IUnitOfWork uow, IAuditLogService audit) { _uow = uow; _audit = audit; }

    public async Task<Result<Guid>> Handle(CreateTowerCommand request, CancellationToken cancellationToken)
    {
        var condominium = _uow.Condominiums.GetById(request.CondominiumId);
        if (condominium is null) return Result<Guid>.Fail("Condominium not found.");

        var duplicate = _uow.Towers.GetByCondominiumId(request.CondominiumId)
            .Any(t => t.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase));
        if (duplicate) return Result<Guid>.Fail("A tower with this name already exists in the condominium.");

        var tower = new Tower { Id = Guid.NewGuid(), Name = request.Name, CondominiumId = request.CondominiumId, CreatedAt = DateTime.UtcNow };
        _uow.Towers.Add(tower);
        await _uow.CommitAsync(cancellationToken);
        await _audit.LogAsync(request.RequestedByUserId, nameof(Tower), tower.Id, "CREATE", cancellationToken: cancellationToken);
        return Result<Guid>.Ok(tower.Id);
    }
}

public class UpdateTowerCommandHandler : IRequestHandler<UpdateTowerCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;

    public UpdateTowerCommandHandler(IUnitOfWork uow, IAuditLogService audit) { _uow = uow; _audit = audit; }

    public async Task<Result<bool>> Handle(UpdateTowerCommand request, CancellationToken cancellationToken)
    {
        var tower = _uow.Towers.GetById(request.Id);
        if (tower is null) return Result<bool>.Fail("Tower not found.");

        tower.Name = request.Name;
        tower.UpdatedAt = DateTime.UtcNow;
        _uow.Towers.Update(tower);
        await _uow.CommitAsync(cancellationToken);
        await _audit.LogAsync(request.RequestedByUserId, nameof(Tower), request.Id, "UPDATE", cancellationToken: cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class DisableTowerCommandHandler : IRequestHandler<DisableTowerCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;

    public DisableTowerCommandHandler(IUnitOfWork uow, IAuditLogService audit) { _uow = uow; _audit = audit; }

    public async Task<Result<bool>> Handle(DisableTowerCommand request, CancellationToken cancellationToken)
    {
        var tower = _uow.Towers.GetById(request.Id);
        if (tower is null) return Result<bool>.Fail("Tower not found.");

        tower.IsDisabled = true;
        tower.UpdatedAt = DateTime.UtcNow;
        _uow.Towers.Update(tower);
        await _uow.CommitAsync(cancellationToken);
        await _audit.LogAsync(request.RequestedByUserId, nameof(Tower), request.Id, "DISABLE", cancellationToken: cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class GetTowersByCondominiumQueryHandler : IRequestHandler<GetTowersByCondominiumQuery, IEnumerable<TowerDto>>
{
    private readonly IUnitOfWork _uow;
    public GetTowersByCondominiumQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<IEnumerable<TowerDto>> Handle(GetTowersByCondominiumQuery request, CancellationToken cancellationToken)
    {
        var result = _uow.Towers.GetByCondominiumId(request.CondominiumId)
            .Select(t => new TowerDto(t.Id, t.Name, t.CondominiumId, t.CreatedAt, t.IsDisabled));
        return Task.FromResult(result);
    }
}

public class GetTowerByIdQueryHandler : IRequestHandler<GetTowerByIdQuery, Result<TowerDto>>
{
    private readonly IUnitOfWork _uow;
    public GetTowerByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<Result<TowerDto>> Handle(GetTowerByIdQuery request, CancellationToken cancellationToken)
    {
        var t = _uow.Towers.GetById(request.Id);
        if (t is null) return Task.FromResult(Result<TowerDto>.Fail("Tower not found."));
        return Task.FromResult(Result<TowerDto>.Ok(new TowerDto(t.Id, t.Name, t.CondominiumId, t.CreatedAt, t.IsDisabled)));
    }
}

