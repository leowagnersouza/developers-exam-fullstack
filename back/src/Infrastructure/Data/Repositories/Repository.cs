using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class Repository<TEntity> : IDisposable, IRepository<TEntity> where TEntity : Entity<TEntity>
{
    protected SqlDbContext Context;
    protected DbSet<TEntity> DbSet;

    public Repository(SqlDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task InsertAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException("entity");

        entity.SetLastAction();

        DbSet.Add(entity);

        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(long id)
    {
        TEntity? entity = await this.GetByIdAsync(id);

        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        DbSet.Remove(entity);

        await Context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException("entity");

        entity.SetLastAction();
        DbSet.Update(entity);

        await Context.SaveChangesAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(long id)
        => await DbSet.FirstOrDefaultAsync(p => p.Id == id);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        => await DbSet.ToListAsync();

    public void Dispose()
        => Context.Dispose();
}