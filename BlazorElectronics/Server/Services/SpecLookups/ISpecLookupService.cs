using BlazorElectronics.Server.Dtos.SpecLookups;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Server.Services.SpecLookups;

public interface ISpecLookupService
{
    Task<ApiReply<CachedSpecLookupData?>> GetSpecLookups();
    Task<ApiReply<List<SpecLookupResponse>?>> GetSpecLookups( PrimaryCategory category );

}