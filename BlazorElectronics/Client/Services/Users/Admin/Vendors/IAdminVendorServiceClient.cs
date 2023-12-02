using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Client.Services.Users.Admin.Vendors;

public interface IAdminVendorServiceClient : IAdminViewService<AdminItemViewDto>
{
    Task<ServiceReply<VendorEditDto?>> GetEdit( IntDto dto );
    Task<ServiceReply<int>> Add( VendorEditDto dto );
    Task<ServiceReply<bool>> Update( VendorEditDto dto );
}