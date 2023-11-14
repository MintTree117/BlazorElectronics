using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Repositories.Specs;

public interface ISpecLookupRepository
{
    Task<SpecLookupsModel?> GetSpecLookupData();
}