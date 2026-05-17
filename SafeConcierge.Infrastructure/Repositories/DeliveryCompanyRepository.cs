using Microsoft.EntityFrameworkCore;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;
using SafeConcierge.Infrastructure.Data;

namespace SafeConcierge.Infrastructure.Repositories;

public class DeliveryCompanyRepository : RepositoryBase<DeliveryCompany>, IDeliveryCompanyRepository
{
    public DeliveryCompanyRepository(AppDbContext context) : base(context)
    {
    }

    public DeliveryCompany? GetByName(string name)
        => DbSet.AsNoTracking()
                .FirstOrDefault(d => d.Name == name);
}

