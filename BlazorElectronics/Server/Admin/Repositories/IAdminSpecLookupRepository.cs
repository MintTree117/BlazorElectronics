using BlazorElectronics.Shared.Admin.Specs;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminSpecLookupRepository
{
    Task<SpecsViewDto?> GetSpecsView();
    Task<EditSpecLookupDto?> GetSpecEdit( GetSpecLookupEditDto dto );
    Task<EditSpecLookupDto?> Insert( AddSpecLookupDto dto );
    Task<bool> Update( EditSpecLookupDto dto );
    Task<bool> Delete( RemoveSpecLookupDto dto );
}