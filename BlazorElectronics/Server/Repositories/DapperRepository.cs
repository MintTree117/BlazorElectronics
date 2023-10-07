using BlazorElectronics.Server.DbContext;

namespace BlazorElectronics.Server.Repositories;

public abstract class DapperRepository<T> : IDapperRepository<T> where T : class
{
    protected readonly DapperContext _dbContext;

    protected DapperRepository( DapperContext dapperContext ) { _dbContext = dapperContext; }

    public abstract Task<IEnumerable<T>?> GetAll();
    public abstract Task<T?> GetById( int id );
}