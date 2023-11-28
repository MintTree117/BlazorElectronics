using BlazorElectronics.Server.Models.Vendors;
using BlazorElectronics.Shared.Admin.Vendors;

namespace BlazorElectronics.Server.Repositories.Vendors;

public interface IVendorRepository
{
    Task<VendorsModel?> Get();
    Task<VendorsViewDto?> GetView();
    Task<VendorEditDto?> GetEdit( int vendorId );
    Task<int> Insert( VendorEditDto dto );
    Task<bool> Update( VendorEditDto dto );
    Task<bool> Delete( int vendorId );
}