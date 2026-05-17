using Microsoft.EntityFrameworkCore;
using SafeConcierge.Domain.Enums;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;
using SafeConcierge.Infrastructure.Data;

namespace SafeConcierge.Infrastructure.Repositories;

public class PackageRepository : RepositoryBase<Package>, IPackageRepository
{
    public PackageRepository(AppDbContext context) : base(context)
    {
    }

    public IEnumerable<Package> GetByCondominiumId(Guid condominiumId)
        => DbSet.AsNoTracking()
                .Where(p => p.CondominiumId == condominiumId)
                .ToList();

    public IEnumerable<Package> GetByResidentId(Guid residentId)
        => DbSet.AsNoTracking()
                .Where(p => p.ResidentId == residentId)
                .ToList();

    public IEnumerable<Package> GetByStatus(PackageStatus status)
        => DbSet.AsNoTracking()
                .Where(p => p.Status == status)
                .ToList();
}

