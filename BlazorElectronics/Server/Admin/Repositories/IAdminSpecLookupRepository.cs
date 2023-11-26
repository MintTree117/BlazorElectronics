using BlazorElectronics.Shared.Admin.SpecLookups;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminSpecLookupRepository
{
    Task<List<SpecLookupViewDto>?> GetView();
    Task<SpecLookupEditDto?> GetEdit( int specId );
    Task<int> Insert( SpecLookupEditDto dto );
    Task<bool> Update( SpecLookupEditDto dto );
    Task<bool> Delete( int specId );
}