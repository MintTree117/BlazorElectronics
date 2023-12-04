using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryService
{
    // USER
    Task<ServiceReply<CategoriesResponse?>> GetCategories();
    Task<ServiceReply<int>> ValidateCategoryUrl( string url );

    // ADMIN
    Task<ServiceReply<List<CategoryViewDto>?>> GetCategoriesView();
    Task<ServiceReply<CategoryEditDto?>> GetCategoryEdit( int categoryId );
    Task<ServiceReply<CategoryEditDto?>> AddCategory( CategoryEditDto dto );
    Task<ServiceReply<bool>> UpdateCategory( CategoryEditDto dto );
    Task<ServiceReply<bool>> RemoveCategory( int categoryId );
}