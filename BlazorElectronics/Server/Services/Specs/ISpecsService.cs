using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Server.Services.Specs;

public interface ISpecsService
{
    Task<ServiceReply<SpecsResponse?>> GetLookups( List<int> primaryCategories );
    Task<ServiceReply<List<CrudView>?>> GetView();
    Task<ServiceReply<SpecEdit?>> GetEdit( int specId );
    Task<ServiceReply<int>> Add( SpecEdit dto );
    Task<ServiceReply<bool>> Update( SpecEdit dto );
    Task<ServiceReply<bool>> Remove( int specId );
}