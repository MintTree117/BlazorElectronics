using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Repositories.Specs;

public interface ISpecLookupRepository : IDapperRepository<SpecLookup>
{
    Task<IEnumerable<SpecLookup>?> GetByCategory( int categoryId );
}