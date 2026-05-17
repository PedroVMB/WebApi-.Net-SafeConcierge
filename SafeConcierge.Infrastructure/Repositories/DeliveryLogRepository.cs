using Microsoft.EntityFrameworkCore;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;
using SafeConcierge.Infrastructure.Data;

namespace SafeConcierge.Infrastructure.Repositories;

public class DeliveryLogRepository : RepositoryBase<DeliveryLog>, IDeliveryLogRepository
{
    public DeliveryLogRepository(AppDbContext context) : base(context)
    {
    }

    public IEnumerable<DeliveryLog> GetByPackageId(Guid packageId)
        => DbSet.AsNoTracking()
                .Where(d => d.PackageId == packageId)
                .ToList();

    public IEnumerable<DeliveryLog> GetByDeliveredToId(Guid userId)
        => DbSet.AsNoTracking()
                .Where(d => d.DeliveredToId == userId)
                .ToList();
}

