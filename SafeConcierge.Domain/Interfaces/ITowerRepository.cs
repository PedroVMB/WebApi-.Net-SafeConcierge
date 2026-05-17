using SafeConcierge.Domain.Models;

namespace SafeConcierge.Domain.Interfaces;

public interface ITowerRepository : IRepositoryBase<Tower>
{
    IEnumerable<Tower> GetByCondominiumId(Guid condominiumId);
}
