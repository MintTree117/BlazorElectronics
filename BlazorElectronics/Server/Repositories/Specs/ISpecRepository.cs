using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Repositories.Specs;

public interface ISpecRepository
{
    Task<IEnumerable<SpecDescr>?> GetAllSpecDescrs();
    Task<IEnumerable<SpecDescr>?> GetSpecDescrsByCategory( int categoryId );
    Task<IEnumerable<SpecLookup>?> GetAllSpecLookups();
    Task<IEnumerable<SpecLookup>?> GetSpecLookupsByCategory( int categoryId );
}