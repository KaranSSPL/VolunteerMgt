using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Abstraction.Entity;
using VolunteerMgt.Server.Abstraction.Persistence;

namespace VolunteerMgt.Server.Persistence;

public class BaseRepository<T, TKey>(DatabaseContext dbContext) : IBaseRepository<T, TKey> where T : class, IBaseEntity<TKey>, IAuditableEntity
{
    public virtual IQueryable<T> All()
    {
        IQueryable<T> query = dbContext.Set<T>();
        return query;
    }

    public virtual void Add(T entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        dbContext.Set<T>().Add(entity);
    }

    public virtual void AddRange(List<T> entities)
    {
        entities.ForEach(e => e.CreatedDate = DateTime.UtcNow);
        dbContext.Set<T>().AddRange(entities);
    }

    public virtual void Update(T entityToUpdate)
    {
        dbContext.Set<T>().Attach(entityToUpdate);
        dbContext.Entry(entityToUpdate).State = EntityState.Modified;
    }

    public virtual void Delete(T entityToDelete)
    {
        if (dbContext.Entry(entityToDelete).State == EntityState.Detached)
        {
            dbContext.Set<T>().Attach(entityToDelete);
        }
        dbContext.Set<T>().Remove(entityToDelete);
    }

    public virtual void DeleteRange(List<T> entities)
    {
        foreach (var entity in entities.Where(e => dbContext.Entry(e).State == EntityState.Detached))
        {
            dbContext.Set<T>().Attach(entity);
        }

        dbContext.Set<T>().RemoveRange(entities);
    }
}