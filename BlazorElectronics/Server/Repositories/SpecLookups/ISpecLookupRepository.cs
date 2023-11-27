using BlazorElectronics.Server.Models.SpecLookups;
using BlazorElectronics.Shared.Admin.SpecLookups;

namespace BlazorElectronics.Server.Repositories.SpecLookups;

public interface ISpecLookupRepository
{
    Task<SpecLookupsModel?> Get();
    Task<List<SpecLookupViewDto>?> GetView();
    Task<SpecLookupEditDto?> GetEdit( int specId );
    Task<int> Insert( SpecLookupEditDto dto );
    Task<bool> Update( SpecLookupEditDto dto );
    Task<bool> Delete( int specId );
}