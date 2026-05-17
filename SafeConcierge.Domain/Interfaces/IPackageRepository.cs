using SafeConcierge.Domain.Enums;
using SafeConcierge.Domain.Models;

namespace SafeConcierge.Domain.Interfaces;

public interface IPackageRepository : IRepositoryBase<Package>
{
    IEnumerable<Package> GetByCondominiumId(Guid condominiumId);
    IEnumerable<Package> GetByResidentId(Guid residentId);
    IEnumerable<Package> GetByStatus(PackageStatus status);
}

