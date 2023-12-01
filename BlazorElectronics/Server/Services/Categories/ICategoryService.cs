using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryService
{
    // USER
    Task<ServiceReply<CategoriesResponse?>> GetCategoriesResponse();
    Task<ServiceReply<CategoryIdMap?>> GetCategoryIdMapFromUrl( string primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null );
    Task<ServiceReply<bool>> ValidateCategoryIdMap( CategoryIdMap idMap );
    
    // ADMIN
    Task<ServiceReply<CategoriesViewDto?>> GetCategoriesView();
    Task<ServiceReply<CategoryEditDto?>> GetCategoryEdit( CategoryGetEditDto dto );
    Task<ServiceReply<CategoryEditDto?>> AddCategory( CategoryAddDto dto );
    Task<ServiceReply<bool>> UpdateCategory( CategoryEditDto dto );
    Task<ServiceReply<bool>> RemoveCategory( CategoryRemoveDto dto );
}