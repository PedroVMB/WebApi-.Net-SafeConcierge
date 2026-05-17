using SafeConcierge.Domain.Models;

namespace SafeConcierge.Domain.Interfaces;

public interface IApartmentRepository : IRepositoryBase<Apartment>
{
    IEnumerable<Apartment> GetByTowerId(Guid towerId);
}
