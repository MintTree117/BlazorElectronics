namespace BlazorElectronics.Server.Repositories;

public interface IDapperRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<T> GetById( int id );
}