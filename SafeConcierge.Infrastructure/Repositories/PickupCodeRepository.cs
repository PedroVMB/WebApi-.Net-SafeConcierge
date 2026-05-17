using Microsoft.EntityFrameworkCore;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;
using SafeConcierge.Infrastructure.Data;

namespace SafeConcierge.Infrastructure.Repositories;

public class PickupCodeRepository : RepositoryBase<PickupCode>, IPickupCodeRepository
{
    public PickupCodeRepository(AppDbContext context) : base(context)
    {
    }

    public PickupCode? GetByCode(string code)
        => DbSet.AsNoTracking()
                .FirstOrDefault(p => p.Code == code);

    public IEnumerable<PickupCode> GetByPackageId(Guid packageId)
        => DbSet.AsNoTracking()
                .Where(p => p.PackageId == packageId)
                .ToList();
}

