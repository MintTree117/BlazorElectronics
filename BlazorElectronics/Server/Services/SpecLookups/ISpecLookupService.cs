using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Server.Services.SpecLookups;

public interface ISpecLookupService
{
    Task<ApiReply<SpecLookupsResponse?>> GetLookups();
    Task<ApiReply<SpecLookupViewResponse?>> GetView();
    Task<ApiReply<SpecLookupEditDto?>> GetEdit( int specId );
    Task<ApiReply<int>> Add( SpecLookupEditDto dto );
    Task<ApiReply<bool>> Update( SpecLookupEditDto dto );
    Task<ApiReply<bool>> Remove( int specId );
}