using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Repositories.Vendors;

public interface IVendorRepository
{
    Task<VendorsResponse?> GetVendors();
}