using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Repositories.Specs;

public interface ISpecRepository
{
    Task<IEnumerable<SpecDescr>?> GetSpecDescrs();
    Task<IEnumerable<SpecLookup>?> GetSpecLookups();
}