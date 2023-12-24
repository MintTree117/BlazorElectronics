using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IVendorService
{
    Task<ServiceReply<VendorsDto?>> GetVendors();
    Task<ServiceReply<List<CrudViewDto>?>> GetView();
    Task<ServiceReply<VendorEditDtoDto?>> GetEdit( int vendorId );
    Task<ServiceReply<int>> Add( VendorEditDtoDto dtoDto );
    Task<ServiceReply<bool>> Update( VendorEditDtoDto dtoDto );
    Task<ServiceReply<bool>> Remove( int vendorId );
}