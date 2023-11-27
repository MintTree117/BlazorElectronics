using BlazorElectronics.Server.Dtos.Categories;
using BlazorElectronics.Server.Dtos.SpecLookups;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminProductSeedRepository
{
    Task<bool> Insert( int amount, CachedCategories cachedCategories, CachedSpecLookupData specLookups );
}