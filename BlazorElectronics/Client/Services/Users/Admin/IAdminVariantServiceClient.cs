using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Variants;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminVariantServiceClient
{
    Task<ApiReply<List<VariantViewDto>?>> GetView();
    Task<ApiReply<VariantEditDto?>> GetEdit( IdDto data );
    Task<ApiReply<int>> Add( VariantAddDto data );
    Task<ApiReply<bool>> Update( VariantEditDto data );
    Task<ApiReply<bool>> Remove( IdDto data );
}