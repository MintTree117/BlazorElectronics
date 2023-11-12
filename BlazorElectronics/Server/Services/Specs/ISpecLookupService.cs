using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Outbound.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public interface ISpecLookupService
{
    Task<ApiReply<CachedSpecData?>> GetSpecDataDto();
    Task<ApiReply<SpecFiltersResponse?>> GetSpecFiltersResponse( short? primaryCategoryId = null );
    Task<ApiReply<bool>> ValidateProductSearchRequestSpecFilters( ProductSearchRequestSpecFilters specFilters, short? primaryCategoryId = null );
}