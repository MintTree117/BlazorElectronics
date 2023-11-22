using BlazorElectronics.Shared.Admin.Specs;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminSpecLookupRepository
{
    Task<SpecsViewDto?> GetSpecsView();
    Task<EditSpecLookupDto?> GetSpecEdit( GetSpecLookupEditDto dto );
    Task<int> Insert( EditSpecLookupDto dto );
    Task<bool> Update( EditSpecLookupDto dto );
    Task<bool> Delete( RemoveSpecLookupDto dto );
}