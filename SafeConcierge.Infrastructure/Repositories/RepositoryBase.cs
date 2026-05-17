using Microsoft.EntityFrameworkCore;
using SafeConcierge.Domain.Interfaces;
using SafeConcierge.Infrastructure.Data;

namespace SafeConcierge.Infrastructure.Repositories;

public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected RepositoryBase(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public void Add(TEntity entity) => DbSet.Add(entity);

    public void Update(TEntity entity) => DbSet.Update(entity);

    public void Delete(TEntity entity) => DbSet.Remove(entity);

    public IEnumerable<TEntity> GetAll() => DbSet.AsNoTracking().ToList();

    public virtual TEntity GetById(Guid id) => DbSet.Find(id)!;
}

