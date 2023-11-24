using BlazorElectronics.Shared.Admin.Specs;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminSpecLookupRepository
{
    Task<SpecsViewDto?> GetView();
    Task<SpecLookupEditDto?> GetEdit( SpecLookupGetEditDto dto );
    Task<int> Insert( SpecLookupEditDto dto );
    Task<bool> Update( SpecLookupEditDto dto );
    Task<bool> Delete( SpecLookupRemoveDto dto );
}