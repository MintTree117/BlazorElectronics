using BlazorElectronics.Shared;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminSpecsServiceClient
{
    Task<ServiceReply<List<SpecLookupViewDto>?>> GetView();
    Task<ServiceReply<SpecLookupEditDto?>> GetEdit( IntDto dto );
    Task<ServiceReply<int>> Add( SpecLookupEditDto dto );
    Task<ServiceReply<bool>> Update( SpecLookupEditDto data );
    Task<ServiceReply<bool>> Remove( IntDto dto );
}