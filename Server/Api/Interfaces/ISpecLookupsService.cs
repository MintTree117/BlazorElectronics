using BlazorElectronics.Shared.Specs;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface ISpecLookupsService
{
    Task<ServiceReply<LookupSpecsDto?>> Get();
    Task<ServiceReply<List<CrudViewDto>?>> GetView();
    Task<ServiceReply<LookupSpecEditDto?>> GetEdit( int specId );
    Task<ServiceReply<int>> Add( LookupSpecEditDto dto );
    Task<ServiceReply<bool>> Update( LookupSpecEditDto dto );
    Task<ServiceReply<bool>> Remove( int specId );
}