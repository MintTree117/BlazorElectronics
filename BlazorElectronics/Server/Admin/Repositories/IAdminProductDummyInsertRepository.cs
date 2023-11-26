using BlazorElectronics.Server.Admin.Models.Variants;
using BlazorElectronics.Server.Dtos.Categories;
using BlazorElectronics.Server.Dtos.SpecLookups;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminProductDummyInsertRepository
{
    Task<bool> Insert( int amount, CachedCategories cachedCategories, VariantsModel variants, CachedSpecLookupData specLookups );
}