using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Repositories.Specs;

public interface ISpecLookupRepository
{
    Task<SpecLookupsModel?> GetSpecLookupDataRound1();
    Task<SpecLookupValuesModel?> GetSpecLookupDataRound2( IEnumerable<string> tableNames );
}