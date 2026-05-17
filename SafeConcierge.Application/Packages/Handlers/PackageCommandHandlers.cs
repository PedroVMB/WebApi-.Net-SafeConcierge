using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Application.Packages.Commands;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Enums;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;

namespace SafeConcierge.Application.Packages.Handlers;

public class RegisterPackageCommandHandler : IRequestHandler<RegisterPackageCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly IPickupCodeService _pickupCodeService;
    private readonly IAuditLogService _audit;

    public RegisterPackageCommandHandler(IUnitOfWork uow, IPickupCodeService pickupCodeService, IAuditLogService audit)
    {
        _uow = uow;
        _pickupCodeService = pickupCodeService;
        _audit = audit;
    }

    public async Task<Result<Guid>> Handle(RegisterPackageCommand request, CancellationToken cancellationToken)
    {
        // RN001 — validar entidades relacionadas
        if (_uow.Apartments.GetById(request.ApartmentId) is null)
            return Result<Guid>.Fail("Apartment not found.");
        if (_uow.Users.GetById(request.ResidentId) is null)
            return Result<Guid>.Fail("Resident not found.");

        var package = new Package
        {
            Id = Guid.NewGuid(),
            CondominiumId = request.CondominiumId,
            ApartmentId = request.ApartmentId,
            ResidentId = request.ResidentId,
            DeliveryCompanyId = request.DeliveryCompanyId,
            ReceivedById = request.ReceivedById,
            Description = request.Description,
            ReceivedAt = request.ReceivedAt,
            Status = PackageStatus.RECEIVED,
            CreatedAt = DateTime.UtcNow
        };

        _uow.Packages.Add(package);
        await _uow.CommitAsync(cancellationToken);

        // RN002 — gerar código temporário
        var pickupCode = await _pickupCodeService.GenerateAsync(package.Id, cancellationToken);

        // Atualizar status para WAITING_PICKUP
        package = _uow.Packages.GetById(package.Id)!;
        package.Status = PackageStatus.WAITING_PICKUP;
        package.UpdatedAt = DateTime.UtcNow;
        _uow.Packages.Update(package);

        // RN003 — criar notificação para o morador
        var company = package.DeliveryCompanyId.HasValue
            ? _uow.DeliveryCompanies.GetById(package.DeliveryCompanyId.Value)?.Name ?? "N/A"
            : "N/A";

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.ResidentId,
            PackageId = package.Id,
            Type = NotificationType.PACKAGE_RECEIVED,
            Content = $"Sua encomenda de {company} foi recebida em {request.ReceivedAt:dd/MM/yyyy HH:mm}. Código de retirada: {pickupCode.Code}",
            Sent = false,
            CreatedAt = DateTime.UtcNow
        };

        _uow.Notifications.Add(notification);
        await _uow.CommitAsync(cancellationToken);

        await _audit.LogAsync(request.ReceivedById, nameof(Package), package.Id, "REGISTER", cancellationToken: cancellationToken);
        return Result<Guid>.Ok(package.Id);
    }
}

public class UpdatePackageCommandHandler : IRequestHandler<UpdatePackageCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;

    public UpdatePackageCommandHandler(IUnitOfWork uow, IAuditLogService audit) { _uow = uow; _audit = audit; }

    public async Task<Result<bool>> Handle(UpdatePackageCommand request, CancellationToken cancellationToken)
    {
        var package = _uow.Packages.GetById(request.Id);
        if (package is null) return Result<bool>.Fail("Package not found.");
        if (package.Status == PackageStatus.DELIVERED) return Result<bool>.Fail("Cannot update a delivered package.");
        if (package.Status == PackageStatus.CANCELED) return Result<bool>.Fail("Cannot update a canceled package.");

        package.Description = request.Description;
        package.DeliveryCompanyId = request.DeliveryCompanyId;
        package.UpdatedAt = DateTime.UtcNow;
        _uow.Packages.Update(package);
        await _uow.CommitAsync(cancellationToken);
        await _audit.LogAsync(request.RequestedByUserId, nameof(Package), request.Id, "UPDATE", cancellationToken: cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class CancelPackageCommandHandler : IRequestHandler<CancelPackageCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly IPickupCodeService _pickupCodeService;
    private readonly IAuditLogService _audit;

    public CancelPackageCommandHandler(IUnitOfWork uow, IPickupCodeService pickupCodeService, IAuditLogService audit)
    {
        _uow = uow;
        _pickupCodeService = pickupCodeService;
        _audit = audit;
    }

    public async Task<Result<bool>> Handle(CancelPackageCommand request, CancellationToken cancellationToken)
    {
        var package = _uow.Packages.GetById(request.Id);
        if (package is null) return Result<bool>.Fail("Package not found.");
        if (package.Status == PackageStatus.DELIVERED) return Result<bool>.Fail("Cannot cancel a delivered package.");
        if (package.Status == PackageStatus.CANCELED) return Result<bool>.Fail("Package is already canceled.");

        // RN005 — invalidar código
        await _pickupCodeService.InvalidateByPackageIdAsync(package.Id, cancellationToken);

        package.Status = PackageStatus.CANCELED;
        package.UpdatedAt = DateTime.UtcNow;
        _uow.Packages.Update(package);

        // Notificar morador
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = package.ResidentId,
            PackageId = package.Id,
            Type = NotificationType.SECURITY_ALERT,
            Content = $"Sua encomenda foi cancelada. Motivo: {request.Reason}",
            Sent = false,
            CreatedAt = DateTime.UtcNow
        };
        _uow.Notifications.Add(notification);
        await _uow.CommitAsync(cancellationToken);

        await _audit.LogAsync(request.RequestedByUserId, nameof(Package), request.Id, "CANCEL",
            newData: $"{{\"reason\":\"{request.Reason}\"}}", cancellationToken: cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class PickupPackageCommandHandler : IRequestHandler<PickupPackageCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;

    public PickupPackageCommandHandler(IUnitOfWork uow, IAuditLogService audit) { _uow = uow; _audit = audit; }

    public async Task<Result<Guid>> Handle(PickupPackageCommand request, CancellationToken cancellationToken)
    {
        var package = _uow.Packages.GetById(request.PackageId);
        if (package is null) return Result<Guid>.Fail("Package not found.");
        if (package.Status != PackageStatus.WAITING_PICKUP) return Result<Guid>.Fail("Package is not waiting for pickup.");
        if (package.ResidentId != request.ResidentId) return Result<Guid>.Fail("Resident does not match the package.");

        // RN004 — validar código
        var pickup = _uow.PickupCodes.GetByCode(request.Code);
        if (pickup is null || pickup.PackageId != request.PackageId)
            return Result<Guid>.Fail("Invalid pickup code.");
        if (pickup.Used) return Result<Guid>.Fail("Pickup code has already been used.");
        if (pickup.ExpiresAt < DateTime.UtcNow) return Result<Guid>.Fail("Pickup code has expired.");

        // Invalidar código — RN005
        pickup.Used = true;
        pickup.UpdatedAt = DateTime.UtcNow;
        _uow.PickupCodes.Update(pickup);

        // Atualizar package
        package.Status = PackageStatus.DELIVERED;
        package.DeliveredAt = DateTime.UtcNow;
        package.UpdatedAt = DateTime.UtcNow;
        _uow.Packages.Update(package);

        // Criar DeliveryLog — RN006
        var log = new DeliveryLog
        {
            Id = Guid.NewGuid(),
            PackageId = request.PackageId,
            PickupCode = request.Code,
            DeliveredToId = request.ResidentId,
            DeliveredById = request.DeliveredById,
            DeliveredAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        _uow.DeliveryLogs.Add(log);

        // Notificar morador
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.ResidentId,
            PackageId = request.PackageId,
            Type = NotificationType.PACKAGE_DELIVERED,
            Content = "Sua encomenda foi retirada com sucesso.",
            Sent = false,
            CreatedAt = DateTime.UtcNow
        };
        _uow.Notifications.Add(notification);
        await _uow.CommitAsync(cancellationToken);

        await _audit.LogAsync(request.DeliveredById, nameof(Package), request.PackageId, "PICKUP", cancellationToken: cancellationToken);
        return Result<Guid>.Ok(log.Id);
    }
}

public class SendPackageReminderCommandHandler : IRequestHandler<SendPackageReminderCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;

    public SendPackageReminderCommandHandler(IUnitOfWork uow, IAuditLogService audit) { _uow = uow; _audit = audit; }

    public async Task<Result<bool>> Handle(SendPackageReminderCommand request, CancellationToken cancellationToken)
    {
        var package = _uow.Packages.GetById(request.PackageId);
        if (package is null) return Result<bool>.Fail("Package not found.");
        if (package.Status != PackageStatus.WAITING_PICKUP) return Result<bool>.Fail("Package is not waiting for pickup.");

        var code = _uow.PickupCodes.GetByPackageId(request.PackageId)
            .Where(p => !p.Used && p.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefault();

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = package.ResidentId,
            PackageId = package.Id,
            Type = NotificationType.PACKAGE_REMINDER,
            Content = $"Você tem uma encomenda aguardando retirada." + (code is not null ? $" Código: {code.Code}" : ""),
            Sent = false,
            CreatedAt = DateTime.UtcNow
        };
        _uow.Notifications.Add(notification);
        await _uow.CommitAsync(cancellationToken);

        await _audit.LogAsync(request.RequestedByUserId, nameof(Package), request.PackageId, "REMINDER", cancellationToken: cancellationToken);
        return Result<bool>.Ok(true);
    }
}

