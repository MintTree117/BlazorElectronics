using BlazorElectronics.Server.Models.Vendors;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Repositories.Vendors;

public interface IVendorRepository
{
    Task<VendorsModel?> Get();
    Task<VendorEditModel?> GetEdit( int vendorId );
    Task<int> Insert( VendorEditDto dto );
    Task<bool> Update( VendorEditDto dto );
    Task<bool> Delete( int vendorId );
}