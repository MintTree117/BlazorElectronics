using BlazorElectronics.Shared.Admin.Variants;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminVariantRepository
{
    Task<VariantsViewDto?> GetView();
    Task<VariantEditDto?> GetEdit( VariantGetEditDto dto );
    Task<int> Insert( VariantAddDto dto );
    Task<bool> Update( VariantEditDto dto );
    Task<bool> Delete( VariantRemoveDto dto );
}