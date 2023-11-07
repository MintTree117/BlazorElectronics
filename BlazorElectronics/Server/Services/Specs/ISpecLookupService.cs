using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Outbound.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public interface ISpecLookupService
{
    Task<Reply<SpecLookupTableMetaDto?>> GetSpecLookupMeta();
    Task<Reply<SpecLookupsResponse?>> GetSpecLookupsResponse( short? primaryCategoryId = null );
    Task<Reply<bool>> ValidateProductSearchRequestSpecFilters( ProductSearchRequestSpecFilters specFilters, short? primaryCategoryId = null );
}