using SafeConcierge.Domain.Models;

namespace SafeConcierge.Domain.Interfaces;

public interface IUserRepository : IRepositoryBase<User>
{
    User? GetByEmail(string email);
    IEnumerable<User> GetByCondominiumId(Guid condominiumId);
}

