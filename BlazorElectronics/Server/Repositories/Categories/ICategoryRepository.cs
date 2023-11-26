using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Repositories.Categories;

public interface ICategoryRepository
{
    Task<CategoriesModel?> GetCategories();
    Task<IEnumerable<string>?> GetPrimaryCategoryDescriptions();
    Task<string?> GetCategoryDescription( CategoryType categoryType, int categoryId );
}