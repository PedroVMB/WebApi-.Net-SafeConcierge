using Microsoft.EntityFrameworkCore;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Domain.Models;
using SafeConcierge.Infrastructure.Data;

namespace SafeConcierge.Infrastructure.Repositories;

public class CondominiumRepository : RepositoryBase<Condominium>, ICondominiumRepository
{
    public CondominiumRepository(AppDbContext context) : base(context)
    {
    }

    public Condominium? GetByCnpj(string cnpj)
        => DbSet.AsNoTracking()
                .FirstOrDefault(c => c.Cnpj == cnpj);
}

