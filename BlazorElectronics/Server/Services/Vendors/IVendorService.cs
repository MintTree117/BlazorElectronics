using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Services.Vendors;

public interface IVendorService
{
    Task<ServiceReply<VendorsResponse?>> GetVendors();
    Task<ServiceReply<List<CrudView>?>> GetView();
    Task<ServiceReply<VendorEdit?>> GetEdit( int vendorId );
    Task<ServiceReply<int>> Add( VendorEdit dto );
    Task<ServiceReply<bool>> Update( VendorEdit dto );
    Task<ServiceReply<bool>> Remove( int vendorId );
}