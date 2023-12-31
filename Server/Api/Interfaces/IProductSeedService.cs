using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IProductSeedService
{
    Task<ServiceReply<bool>> SeedProducts( int amount, CategoryData categories, LookupSpecsDto lookups, VendorsDto vendors );
}