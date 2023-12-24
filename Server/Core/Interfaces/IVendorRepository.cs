using BlazorElectronics.Server.Core.Models.Vendors;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IVendorRepository
{
    Task<VendorsModel?> Get();
    Task<IEnumerable<VendorDto>?> GetView();
    Task<VendorEditModel?> GetEdit( int vendorId );
    Task<int> Insert( VendorEditDtoDto dtoDto );
    Task<bool> Update( VendorEditDtoDto dtoDto );
    Task<bool> Delete( int vendorId );
}