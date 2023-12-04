using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Server.Services.SpecLookups;

public interface ISpecLookupService
{
    Task<ServiceReply<SpecLookupsResponse?>> GetLookups();
    Task<ServiceReply<List<CrudView>?>> GetView();
    Task<ServiceReply<SpecLookupEdit?>> GetEdit( int specId );
    Task<ServiceReply<int>> Add( SpecLookupEdit dto );
    Task<ServiceReply<bool>> Update( SpecLookupEdit dto );
    Task<ServiceReply<bool>> Remove( int specId );
}