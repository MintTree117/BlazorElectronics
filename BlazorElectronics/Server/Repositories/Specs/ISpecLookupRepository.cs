using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Repositories.Specs;

public interface ISpecLookupRepository
{
    Task<SpecLookupsModel?> GetSpecLookupMeta();
    Task<SpecLookupValuesModel?> GetSpecLookupData( Dictionary<short, string> dynamicTableNamesById );
}