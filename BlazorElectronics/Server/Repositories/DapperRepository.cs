using BlazorElectronics.Server.DbContext;

namespace BlazorElectronics.Server.Repositories;

public abstract class DapperRepository
{
    protected readonly DapperContext _dbContext;

    protected DapperRepository( DapperContext dapperContext ) { _dbContext = dapperContext; }
}