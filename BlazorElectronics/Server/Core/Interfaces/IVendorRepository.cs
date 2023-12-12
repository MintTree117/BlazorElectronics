using BlazorElectronics.Server.Core.Models.Vendors;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IVendorRepository
{
    Task<VendorsModel?> Get();
    Task<IEnumerable<VendorModel>?> GetView();
    Task<VendorEditModel?> GetEdit( int vendorId );
    Task<int> Insert( VendorEdit dto );
    Task<bool> Update( VendorEdit dto );
    Task<bool> Delete( int vendorId );
}