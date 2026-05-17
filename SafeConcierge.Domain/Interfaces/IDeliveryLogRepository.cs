using SafeConcierge.Domain.Models;

namespace SafeConcierge.Domain.Interfaces;

public interface IDeliveryLogRepository : IRepositoryBase<DeliveryLog>
{
    IEnumerable<DeliveryLog> GetByPackageId(Guid packageId);
    IEnumerable<DeliveryLog> GetByDeliveredToId(Guid userId);
}

