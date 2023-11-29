using BlazorElectronics.Shared.Admin.Categories;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryService
{
    // USER
    Task<ApiReply<CategoriesResponse?>> GetCategoriesResponse();
    Task<ApiReply<CategoryIdMap?>> GetCategoryIdMapFromUrl( string primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null );
    Task<ApiReply<bool>> ValidateCategoryIdMap( CategoryIdMap idMap );
    
    // ADMIN
    Task<ApiReply<CategoriesViewDto?>> GetCategoriesView();
    Task<ApiReply<CategoryEditDto?>> GetCategoryEdit( CategoryGetEditDto dto );
    Task<ApiReply<CategoryEditDto?>> AddCategory( CategoryAddDto dto );
    Task<ApiReply<bool>> UpdateCategory( CategoryEditDto dto );
    Task<ApiReply<bool>> RemoveCategory( CategoryRemoveDto dto );
}