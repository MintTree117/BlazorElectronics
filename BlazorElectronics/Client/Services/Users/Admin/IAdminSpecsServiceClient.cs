using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.SpecLookups;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminSpecsServiceClient
{
    Task<ApiReply<List<SpecLookupViewDto>?>> GetView();
    Task<ApiReply<SpecLookupEditDto?>> GetEdit( IdDto dto );
    Task<ApiReply<int>> Add( SpecLookupEditDto dto );
    Task<ApiReply<bool>> Update( SpecLookupEditDto data );
    Task<ApiReply<bool>> Remove( IdDto dto );
}