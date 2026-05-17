using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Application.PickupCodes.Commands;
using SafeConcierge.Application.PickupCodes.Queries;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Interfaces;

namespace SafeConcierge.Application.PickupCodes.Handlers;

public class InvalidatePickupCodeCommandHandler : IRequestHandler<InvalidatePickupCodeCommand, Result<bool>>
{
    private readonly IPickupCodeService _svc;
    private readonly IAuditLogService _audit;

    public InvalidatePickupCodeCommandHandler(IPickupCodeService svc, IAuditLogService audit) { _svc = svc; _audit = audit; }

    public async Task<Result<bool>> Handle(InvalidatePickupCodeCommand request, CancellationToken cancellationToken)
    {
        await _svc.InvalidateByPackageIdAsync(request.PackageId, cancellationToken);
        await _audit.LogAsync(request.RequestedByUserId, "PickupCode", request.PackageId, "INVALIDATE", cancellationToken: cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public class RegeneratePickupCodeCommandHandler : IRequestHandler<RegeneratePickupCodeCommand, Result<PickupCodeDto>>
{
    private readonly IPickupCodeService _svc;
    private readonly IAuditLogService _audit;

    public RegeneratePickupCodeCommandHandler(IPickupCodeService svc, IAuditLogService audit) { _svc = svc; _audit = audit; }

    public async Task<Result<PickupCodeDto>> Handle(RegeneratePickupCodeCommand request, CancellationToken cancellationToken)
    {
        await _svc.InvalidateByPackageIdAsync(request.PackageId, cancellationToken);
        var newCode = await _svc.GenerateAsync(request.PackageId, cancellationToken);
        await _audit.LogAsync(request.RequestedByUserId, "PickupCode", request.PackageId, "REGENERATE", cancellationToken: cancellationToken);
        return Result<PickupCodeDto>.Ok(new PickupCodeDto(newCode.Id, newCode.PackageId, newCode.Code, newCode.ExpiresAt, newCode.Used));
    }
}

public class GetPickupCodeByPackageQueryHandler : IRequestHandler<GetPickupCodeByPackageQuery, Result<PickupCodeDto>>
{
    private readonly IUnitOfWork _uow;
    public GetPickupCodeByPackageQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<Result<PickupCodeDto>> Handle(GetPickupCodeByPackageQuery request, CancellationToken cancellationToken)
    {
        // Residents can only see codes for their own packages
        if (request.RequestedByRole == "RESIDENT")
        {
            var pkg = _uow.Packages.GetById(request.PackageId);
            if (pkg is null || pkg.ResidentId != request.RequestedByUserId)
                return Task.FromResult(Result<PickupCodeDto>.Fail("Access denied."));
        }

        var code = _uow.PickupCodes.GetByPackageId(request.PackageId)
            .Where(c => !c.Used && c.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefault();

        if (code is null) return Task.FromResult(Result<PickupCodeDto>.Fail("No active pickup code found."));
        return Task.FromResult(Result<PickupCodeDto>.Ok(new PickupCodeDto(code.Id, code.PackageId, code.Code, code.ExpiresAt, code.Used)));
    }
}

public class ValidatePickupCodeQueryHandler : IRequestHandler<ValidatePickupCodeQuery, Result<bool>>
{
    private readonly IUnitOfWork _uow;
    public ValidatePickupCodeQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<Result<bool>> Handle(ValidatePickupCodeQuery request, CancellationToken cancellationToken)
    {
        var code = _uow.PickupCodes.GetByCode(request.Code);
        if (code is null || code.PackageId != request.PackageId) return Task.FromResult(Result<bool>.Fail("Invalid code."));
        if (code.Used) return Task.FromResult(Result<bool>.Fail("Code already used."));
        if (code.ExpiresAt < DateTime.UtcNow) return Task.FromResult(Result<bool>.Fail("Code expired."));
        return Task.FromResult(Result<bool>.Ok(true));
    }
}

