using BlazorElectronics.Shared.Specs;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface ISpecsService
{
    Task<ServiceReply<SpecsResponse?>> GetSpecs( List<int> primaryCategoryIds );
    Task<ServiceReply<List<CrudView>?>> GetView();
    Task<ServiceReply<SpecEdit?>> GetEdit( int specId );
    Task<ServiceReply<int>> Add( SpecEdit dto );
    Task<ServiceReply<bool>> Update( SpecEdit dto );
    Task<ServiceReply<bool>> Remove( int specId );
}