using BlazorElectronics.Server.Dtos.Categories;
using BlazorElectronics.Server.Dtos.SpecLookups;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductSeedService
{
    Task<ApiReply<bool>> SeedProducts( int amount, CachedCategories cachedCategories, CachedSpecLookupData specLookups, VendorsResponse vendors, List<int> users );
}