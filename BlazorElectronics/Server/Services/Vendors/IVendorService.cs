using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Services.Vendors;

public interface IVendorService
{
    Task<ApiReply<VendorsResponse?>> GetVendors();
}