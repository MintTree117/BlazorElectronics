using BlazorElectronics.Server.Models.Vendors;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Repositories.Vendors;

public interface IVendorRepository
{
    Task<VendorsModel?> Get();
    Task<IEnumerable<VendorModel>?> GetView();
    Task<VendorEditModel?> GetEdit( int vendorId );
    Task<int> Insert( VendorEdit dto );
    Task<bool> Update( VendorEdit dto );
    Task<bool> Delete( int vendorId );
}