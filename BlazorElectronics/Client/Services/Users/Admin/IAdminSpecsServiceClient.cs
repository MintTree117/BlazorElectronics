using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Specs;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminSpecsServiceClient
{
    Task<ApiReply<SpecsViewDto?>> GetView();
    Task<ApiReply<SpecLookupEditDto?>> GetEdit( SpecLookupGetEditDto dto );
    Task<ApiReply<int>> Add( SpecLookupEditDto dto );
    Task<ApiReply<bool>> Update( SpecLookupEditDto data );
    Task<ApiReply<bool>> Remove( SpecLookupRemoveDto data );
}