using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryService
{
    Task<ApiReply<CategoriesResponse?>> GetCategoriesResponse();
    Task<ApiReply<CategoryIdMap?>> GetCategoryIdMapFromUrl( string primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null );
    Task<ApiReply<bool>> ValidateCategoryIdMap( CategoryIdMap idMap );
}