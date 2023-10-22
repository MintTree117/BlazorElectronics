using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Repositories.Specs;

public interface ISpecDescrRepository : IDapperRepository<SpecDescr>
{
    Task<IEnumerable<SpecDescr>?> GetByCategory( int categoryId );
}