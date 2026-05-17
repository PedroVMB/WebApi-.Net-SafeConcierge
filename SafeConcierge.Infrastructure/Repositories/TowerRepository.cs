using Microsoft.EntityFrameworkCore;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;
using SafeConcierge.Infrastructure.Data;

namespace SafeConcierge.Infrastructure.Repositories;

public class TowerRepository : RepositoryBase<Tower>, ITowerRepository
{
    public TowerRepository(AppDbContext context) : base(context) { }

    public IEnumerable<Tower> GetByCondominiumId(Guid condominiumId)
        => DbSet.AsNoTracking().Where(t => t.CondominiumId == condominiumId).ToList();
}
