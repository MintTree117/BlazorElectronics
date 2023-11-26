using BlazorElectronics.Server.Models.SpecLookups;

namespace BlazorElectronics.Server.Repositories.SpecLookups;

public interface ISpecLookupRepository
{
    Task<SpecLookupsModel?> GetSpecLookupData();
}