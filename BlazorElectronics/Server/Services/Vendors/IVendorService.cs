using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Services.Vendors;

public interface IVendorService
{
    Task<ServiceReply<VendorsResponse?>> GetVendors();
    Task<ServiceReply<List<AdminItemViewDto>?>> GetView();
    Task<ServiceReply<VendorEditDto?>> GetEdit( int vendorId );
    Task<ServiceReply<VendorEditDto?>> Add( VendorEditDto dto );
    Task<ServiceReply<bool>> Update( VendorEditDto dto );
    Task<ServiceReply<bool>> Remove( int vendorId );
}