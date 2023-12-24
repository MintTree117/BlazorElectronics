using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Client.Services.Vendors;

public interface IVendorServiceClient
{
    Task<ServiceReply<VendorsDto?>> GetVendors();
}