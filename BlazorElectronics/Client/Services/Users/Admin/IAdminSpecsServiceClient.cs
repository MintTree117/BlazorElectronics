using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Specs;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminSpecsServiceClient
{
    Task<ApiReply<SpecsViewDto?>> GetView();
    Task<ApiReply<EditSpecLookupDto?>> GetEdit( GetSpecLookupEditDto dto );
    Task<ApiReply<EditSpecLookupDto?>> Add( AddSpecLookupDto dto );
    Task<ApiReply<bool>> Update( EditSpecLookupDto data );
    Task<ApiReply<bool>> Remove( RemoveSpecLookupDto data );
}