using MediatR;
using SafeConcierge.Application.Condominiums.Commands;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;

namespace SafeConcierge.Application.Condominiums.Handlers;

public class CreateCondominiumCommandHandler : IRequestHandler<CreateCondominiumCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;

    public CreateCondominiumCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CreateCondominiumCommand request, CancellationToken cancellationToken)
    {
        var existing = _uow.Condominiums.GetByCnpj(request.Cnpj);
        if (existing is not null) return Result<Guid>.Fail("A condominium with this CNPJ already exists.");

        var condominium = new Condominium
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Cnpj = request.Cnpj,
            Address = request.Address,
            CreatedAt = DateTime.UtcNow
        };

        _uow.Condominiums.Add(condominium);
        await _uow.CommitAsync(cancellationToken);
        return Result<Guid>.Ok(condominium.Id);
    }
}

public class UpdateCondominiumCommandHandler : IRequestHandler<UpdateCondominiumCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;

    public UpdateCondominiumCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(UpdateCondominiumCommand request, CancellationToken cancellationToken)
    {
        var condominium = _uow.Condominiums.GetById(request.Id);
        if (condominium is null) return Result<bool>.Fail("Condominium not found.");

        condominium.Name = request.Name;
        condominium.Cnpj = request.Cnpj;
        condominium.Address = request.Address;
        condominium.UpdatedAt = DateTime.UtcNow;

        _uow.Condominiums.Update(condominium);
        await _uow.CommitAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class DisableCondominiumCommandHandler : IRequestHandler<DisableCondominiumCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;

    public DisableCondominiumCommandHandler(IUnitOfWork uow, IAuditLogService audit)
    {
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<bool>> Handle(DisableCondominiumCommand request, CancellationToken cancellationToken)
    {
        var condominium = _uow.Condominiums.GetById(request.Id);
        if (condominium is null) return Result<bool>.Fail("Condominium not found.");

        condominium.IsDisabled = true;
        condominium.UpdatedAt = DateTime.UtcNow;

        _uow.Condominiums.Update(condominium);
        await _uow.CommitAsync(cancellationToken);

        await _audit.LogAsync(request.RequestedByUserId, nameof(Condominium), request.Id,
            "DISABLE", cancellationToken: cancellationToken);

        return Result<bool>.Ok(true);
    }
}

