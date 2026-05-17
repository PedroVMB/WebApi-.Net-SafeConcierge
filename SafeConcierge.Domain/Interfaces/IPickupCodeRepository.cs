using SafeConcierge.Domain.Models;

namespace SafeConcierge.Domain.Interfaces;

public interface IPickupCodeRepository : IRepositoryBase<PickupCode>
{
    PickupCode? GetByCode(string code);
    IEnumerable<PickupCode> GetByPackageId(Guid packageId);
}

