using BlazorElectronics.Shared.Admin.Vendors;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminVendorRepository
{
    Task<VendorsViewDto?> GetView();
    Task<VendorEditDto?> GetEdit( int vendorId );
    Task<int> Insert( VendorEditDto dto );
    Task<bool> Update( VendorEditDto dto );
    Task<bool> Delete( int vendorId );
}