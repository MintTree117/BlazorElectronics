using BlazorElectronics.Shared.Admin.Vendors;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Services.Vendors;

public interface IVendorService
{
    Task<ApiReply<VendorsResponse?>> GetVendors();
    Task<ApiReply<VendorsViewDto?>> GetView();
    Task<ApiReply<VendorEditDto?>> GetEdit( int vendorId );
    Task<ApiReply<int>> Add( VendorEditDto dto );
    Task<ApiReply<bool>> Update( VendorEditDto dto );
    Task<ApiReply<bool>> Remove( int vendorId );
}