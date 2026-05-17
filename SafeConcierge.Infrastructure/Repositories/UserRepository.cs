using Microsoft.EntityFrameworkCore;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;
using SafeConcierge.Infrastructure.Data;

namespace SafeConcierge.Infrastructure.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public override User GetById(Guid id)
        => DbSet.Find(id)!;

    public User? GetByEmail(string email)
        => DbSet.AsNoTracking()
                .FirstOrDefault(u => u.Email == email);

    public IEnumerable<User> GetByCondominiumId(Guid condominiumId)
        => DbSet.AsNoTracking()
                .Where(u => u.CondominiumId == condominiumId)
                .ToList();
}

