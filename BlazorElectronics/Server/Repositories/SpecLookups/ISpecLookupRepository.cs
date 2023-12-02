using BlazorElectronics.Server.Models.SpecLookups;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Server.Repositories.SpecLookups;

public interface ISpecLookupRepository
{
    Task<SpecLookupsModel?> Get();
    Task<IEnumerable<SpecLookupModel>?> GetView();
    Task<SpecLookupEditModel?> GetEdit( int specId );
    Task<SpecLookupEditModel?> Insert( SpecLookupEditDto dto );
    Task<bool> Update( SpecLookupEditDto dto );
    Task<bool> Delete( int specId );
}