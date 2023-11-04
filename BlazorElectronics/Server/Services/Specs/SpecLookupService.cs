using BlazorElectronics.Shared.Outbound.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public class SpecLookupService : ISpecLookupService
{
    public Task<Reply<SpecLookupsGlobalResponse>> GetSpecLookupsGlobal()
    {
        throw new NotImplementedException();
    }
    public Task<Reply<SpecsLookupsCategoryResponse>> GetSpecLookupsCategory( int primaryCategoryId )
    {
        throw new NotImplementedException();
    }
}