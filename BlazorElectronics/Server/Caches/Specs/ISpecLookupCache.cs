using BlazorElectronics.Shared.Outbound.Specs;

namespace BlazorElectronics.Server.Caches.Specs;

public interface ISpecLookupCache
{
    Task<SpecLookupsGlobalResponse?> GetSpecLookupsGlobal();
    Task<SpecsLookupsCategoryResponse?> GetSpecLookupsCategory( int primaryCategoryId );

    Task SetSpecLookupsGlobal( SpecLookupsGlobalResponse response );
    Task SetSpecLookupsCategory( int primaryCategoryId, SpecsLookupsCategoryResponse respose );
}