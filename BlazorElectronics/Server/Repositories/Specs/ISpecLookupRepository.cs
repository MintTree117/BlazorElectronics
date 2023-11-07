using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Repositories.Specs;

public interface ISpecLookupRepository
{
    Task<SpecLookupTableMetaModel?> GetSpecTableMeta();
    Task<SpecLookupDataModel?> GetSpecLookupData( SpecLookupTableMetaModel meta );
    Task<DynamicSpecLookups?> GetDynamicSpecLookups( Dictionary<short, string> tableNames );
}