using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminVendorServiceClient
{
    Task<ServiceReply<VendorsViewDto?>> GetView();
    Task<ServiceReply<VendorEditDto?>> GetEdit( IntDto dto );
    Task<ServiceReply<int>> Add( VendorEditDto dto );
    Task<ServiceReply<bool>> Update( VendorEditDto dto );
    Task<ServiceReply<bool>> Remove( IntDto dto );
}