using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductSeedService
{
    Task<ServiceReply<bool>> SeedProducts( int amount, CategoryData categories, SpecsResponse lookups, VendorsResponse vendors, List<int> users );
}