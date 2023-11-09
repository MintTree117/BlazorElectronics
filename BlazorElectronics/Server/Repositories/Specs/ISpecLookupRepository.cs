using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Repositories.Specs;

public interface ISpecLookupRepository
{
    Task<SpecLookupMetaModel?> GetSpecLookupMeta();
    Task<DynamicSpecLookupValuesModel?> GetSpecLookupData( Dictionary<short, string> dynamicTableNamesById );
}