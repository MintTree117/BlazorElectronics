using BlazorElectronics.Shared;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Client.Services.SpecLookups;

public interface ISpecLookupServiceClient
{
    Task<ServiceReply<SpecLookupsResponse?>> GetSpecLookups();
}