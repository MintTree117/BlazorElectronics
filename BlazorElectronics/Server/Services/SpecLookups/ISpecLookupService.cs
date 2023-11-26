using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Server.Services.SpecLookups;

public interface ISpecLookupService
{
    Task<ApiReply<List<SpecLookupResponse>?>> GetSpecLookups();
    Task<ApiReply<List<SpecLookupResponse>?>> GetSpecLookups( PrimaryCategory category );

}