using BlazorElectronics.Server.Dtos.Categories;
using BlazorElectronics.Shared.Mutual;
using BlazorElectronics.Shared.Outbound.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryService
{
    Task<ApiReply<CategoriesDto?>> GetCategoriesDto();
    Task<ApiReply<CategoriesResponse?>> GetCategories();
    Task<ApiReply<IReadOnlyList<string>?>> GetMainDescriptions();
    Task<ApiReply<string?>> GetDescription( CategoryIdMap? idMap );
    Task<ApiReply<CategoryIdMap?>> GetCategoryIdMapFromUrl( string primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null );
    Task<ApiReply<bool>> ValidateCategoryIdMap( CategoryIdMap? idMap );
}