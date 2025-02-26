using VolunteerMgt.Server.Abstraction.Persistence;

namespace VolunteerMgt.Server.Persistence;

public sealed class UnitOfWork(DatabaseContext dbContext) : IDisposable, IUnitOfWork
{
    private bool _isDisposed;

    //public IBaseRepository<EntityName, EntityKeyType> EntityNames { get; } = new BaseRepository<EntityName, EntityKeyType>(dbContext);

    public async Task<int> SaveChangesAsync() => await dbContext.SaveChangesAsync();

    private void Dispose(bool disposing)
    {
        if (!_isDisposed && disposing)
        {
            dbContext.Dispose();
        }
        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
