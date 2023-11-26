using BlazorElectronics.Shared.Admin.Variants;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminVariantRepository
{
    Task<List<VariantViewDto>?> GetView();
    Task<VariantEditDto?> GetEdit( int id );
    Task<int> Insert( VariantAddDto dto );
    Task<bool> Update( VariantEditDto dto );
    Task<bool> Delete( int id );
}