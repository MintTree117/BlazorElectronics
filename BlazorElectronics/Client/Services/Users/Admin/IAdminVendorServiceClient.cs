using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Vendors;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminVendorServiceClient
{
    Task<ApiReply<VendorsViewDto?>> GetView();
    Task<ApiReply<VendorEditDto?>> GetEdit( IntDto dto );
    Task<ApiReply<int>> Add( VendorEditDto dto );
    Task<ApiReply<bool>> Update( VendorEditDto dto );
    Task<ApiReply<bool>> Remove( IntDto dto );
}