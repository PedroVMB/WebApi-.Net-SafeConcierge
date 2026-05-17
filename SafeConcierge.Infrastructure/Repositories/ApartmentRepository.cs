using Microsoft.EntityFrameworkCore;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;
using SafeConcierge.Infrastructure.Data;

namespace SafeConcierge.Infrastructure.Repositories;

public class ApartmentRepository : RepositoryBase<Apartment>, IApartmentRepository
{
    public ApartmentRepository(AppDbContext context) : base(context) { }

    public IEnumerable<Apartment> GetByTowerId(Guid towerId)
        => DbSet.AsNoTracking().Where(a => a.TowerId == towerId).ToList();
}
