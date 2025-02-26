using VolunteerMgt.Server.Abstraction.Entity;

namespace VolunteerMgt.Server.Abstraction.Persistence;

public interface IBaseRepository<T, TKey> where T : class, IBaseEntity<TKey>
{
    IQueryable<T> All();
    void Add(T entity);
    void AddRange(List<T> entities);
    void Update(T entityToUpdate);
    void Delete(T entityToDelete);
    void DeleteRange(List<T> entities);
}