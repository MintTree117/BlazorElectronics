using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public class SpecService : ISpecService
{
    public Task<ServiceResponse<SpecMetaData>> GetSpecMetaData() { throw new NotImplementedException(); }
    public Task<ServiceResponse<Dictionary<int, List<object>>>> GetSpecLookups() { throw new NotImplementedException(); }
}