using BlazorElectronics.Server.Dtos.Categories;
using BlazorElectronics.Server.Dtos.Specs;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminProductDummyInsertRepository
{
    Task<bool> Insert( int amount, CategoriesDto categoryData, CachedSpecData specLookupData );
}