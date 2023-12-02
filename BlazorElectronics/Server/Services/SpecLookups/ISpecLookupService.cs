using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Server.Services.SpecLookups;

public interface ISpecLookupService
{
    Task<ServiceReply<SpecLookupsResponse?>> GetLookups();
    Task<ServiceReply<SpecLookupViewResponse?>> GetView();
    Task<ServiceReply<SpecLookupEditDto?>> GetEdit( int specId );
    Task<ServiceReply<SpecLookupEditDto?>> Add( SpecLookupEditDto dto );
    Task<ServiceReply<bool>> Update( SpecLookupEditDto dto );
    Task<ServiceReply<bool>> Remove( int specId );
}