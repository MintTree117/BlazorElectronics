using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.SpecLookups;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductSeedService
{
    Task<ServiceReply<bool>> SeedProducts( int amount, CategoriesResponse categories, SpecLookupsResponse lookups, VendorsResponse vendors, List<int> users );
}