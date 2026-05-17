using MediatR;
using SafeConcierge.Application.Common.DTOs;
using SafeConcierge.Application.Packages.Queries;
using SafeConcierge.Domain.Common;
using SafeConcierge.Domain.Enums;
using SafeConcierge.Domain.Interfaces;

namespace SafeConcierge.Application.Packages.Handlers;

public class GetPackageByIdQueryHandler : IRequestHandler<GetPackageByIdQuery, Result<PackageDetailDto>>
{
    private readonly IUnitOfWork _uow;
    public GetPackageByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<Result<PackageDetailDto>> Handle(GetPackageByIdQuery request, CancellationToken cancellationToken)
    {
        var p = _uow.Packages.GetById(request.Id);
        if (p is null) return Task.FromResult(Result<PackageDetailDto>.Fail("Package not found."));

        // Residents only see their own packages
        if (request.RequestedByRole == "RESIDENT" && p.ResidentId != request.RequestedByUserId)
            return Task.FromResult(Result<PackageDetailDto>.Fail("Access denied."));

        var resident = _uow.Users.GetById(p.ResidentId);
        var apartment = _uow.Apartments.GetById(p.ApartmentId);
        var company = p.DeliveryCompanyId.HasValue ? _uow.DeliveryCompanies.GetById(p.DeliveryCompanyId.Value) : null;
        var receivedBy = _uow.Users.GetById(p.ReceivedById);

        var activeCode = _uow.PickupCodes.GetByPackageId(p.Id)
            .Where(c => !c.Used && c.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefault();

        var codeDto = activeCode is not null
            ? new PickupCodeDto(activeCode.Id, activeCode.PackageId, activeCode.Code, activeCode.ExpiresAt, activeCode.Used)
            : null;

        var dto = new PackageDetailDto(
            p.Id, p.CondominiumId, p.ApartmentId, p.ResidentId,
            p.DeliveryCompanyId, p.ReceivedById, p.Status.ToString(),
            p.Description, p.ReceivedAt, p.DeliveredAt,
            resident?.Name, apartment?.Number, company?.Name, receivedBy?.Name, codeDto);

        return Task.FromResult(Result<PackageDetailDto>.Ok(dto));
    }
}

public class GetPackagesByCondominiumQueryHandler : IRequestHandler<GetPackagesByCondominiumQuery, PagedResult<PackageDto>>
{
    private readonly IUnitOfWork _uow;
    public GetPackagesByCondominiumQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<PagedResult<PackageDto>> Handle(GetPackagesByCondominiumQuery request, CancellationToken cancellationToken)
    {
        var query = _uow.Packages.GetByCondominiumId(request.CondominiumId).AsQueryable();
        if (request.Status.HasValue) query = query.Where(p => p.Status == request.Status.Value);

        var total = query.Count();
        var items = query
            .OrderByDescending(p => p.ReceivedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => ToDto(p))
            .ToList();

        return Task.FromResult(new PagedResult<PackageDto>(items, total, request.Page, request.PageSize));
    }

    private PackageDto ToDto(Domain.Models.Package p) =>
        new(p.Id, p.CondominiumId, p.ApartmentId, p.ResidentId,
            p.DeliveryCompanyId, p.ReceivedById, p.Status.ToString(),
            p.Description, p.ReceivedAt, p.DeliveredAt, null, null, null);
}

public class GetMyPackagesQueryHandler : IRequestHandler<GetMyPackagesQuery, IEnumerable<PackageDto>>
{
    private readonly IUnitOfWork _uow;
    public GetMyPackagesQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<IEnumerable<PackageDto>> Handle(GetMyPackagesQuery request, CancellationToken cancellationToken)
    {
        var query = _uow.Packages.GetByResidentId(request.ResidentId).AsQueryable();
        if (request.Status.HasValue) query = query.Where(p => p.Status == request.Status.Value);

        var result = query.OrderByDescending(p => p.ReceivedAt)
            .Select(p => new PackageDto(p.Id, p.CondominiumId, p.ApartmentId, p.ResidentId,
                p.DeliveryCompanyId, p.ReceivedById, p.Status.ToString(),
                p.Description, p.ReceivedAt, p.DeliveredAt, null, null, null))
            .ToList();

        return Task.FromResult<IEnumerable<PackageDto>>(result);
    }
}

public class GetPendingPackagesQueryHandler : IRequestHandler<GetPendingPackagesQuery, IEnumerable<PackageDto>>
{
    private readonly IUnitOfWork _uow;
    public GetPendingPackagesQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<IEnumerable<PackageDto>> Handle(GetPendingPackagesQuery request, CancellationToken cancellationToken)
    {
        var result = _uow.Packages.GetByStatus(PackageStatus.WAITING_PICKUP)
            .Where(p => p.CondominiumId == request.CondominiumId)
            .OrderBy(p => p.ReceivedAt)
            .Select(p => new PackageDto(p.Id, p.CondominiumId, p.ApartmentId, p.ResidentId,
                p.DeliveryCompanyId, p.ReceivedById, p.Status.ToString(),
                p.Description, p.ReceivedAt, p.DeliveredAt, null, null, null))
            .ToList();

        return Task.FromResult<IEnumerable<PackageDto>>(result);
    }
}

