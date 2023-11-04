using BlazorElectronics.Shared.Outbound.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public interface ISpecLookupService
{
    Task<Reply<SpecLookupsGlobalResponse>> GetSpecLookupsGlobal();
    Task<Reply<SpecsLookupsCategoryResponse>> GetSpecLookupsCategory( int primaryCategoryId );
}