using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryService
{
    // USER
    Task<ServiceReply<CategoriesResponse?>> GetCategories();
    Task<ServiceReply<int>> ValidateCategoryUrl( string url );

    // ADMIN
    Task<ServiceReply<List<CategoryView>?>> GetCategoriesView();
    Task<ServiceReply<CategoryEdit?>> GetCategoryEdit( int categoryId );
    Task<ServiceReply<bool>> AddBulkCategories( List<CategoryEdit> categories );
    Task<ServiceReply<int>> AddCategory( CategoryEdit dto );
    Task<ServiceReply<bool>> UpdateCategory( CategoryEdit dto );
    Task<ServiceReply<bool>> RemoveCategory( int categoryId );
}