using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Specs;

namespace BlazorElectronics.Client.Services.Specs;

public interface ISpecServiceClient
{
    Task<ServiceReply<LookupSpecsDto?>> GetSpecLookups();
}