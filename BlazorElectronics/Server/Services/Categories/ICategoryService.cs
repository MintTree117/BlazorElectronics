using BlazorElectronics.Shared.Mutual;
using BlazorElectronics.Shared.Outbound.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryService
{
    Task<Reply<CategoriesResponse?>> GetCategories();
    Task<Reply<IReadOnlyList<string>?>> GetMainDescriptions();
    Task<Reply<string?>> GetDescription( CategoryIdMap? idMap );
    Task<Reply<CategoryIdMap?>> GetCategoryIdMapFromUrl( string primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null );
    Task<Reply<bool>> ValidateCategoryIdMap( CategoryIdMap? idMap );
}