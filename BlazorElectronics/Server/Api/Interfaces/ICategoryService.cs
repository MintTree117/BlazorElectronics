using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface ICategoryService
{
    // USER
    Task<ServiceReply<List<int>?>> GetPrimaryCategoryIds(); 
    Task<ServiceReply<CategoryData?>> GetCategoryData();
    Task<ServiceReply<List<CategoryResponse>?>> GetCategoryResponse();
    Task<ServiceReply<int>> ValidateCategoryUrl( string url );

    // ADMIN
    Task<ServiceReply<List<CategoryView>?>> GetCategoriesView();
    Task<ServiceReply<CategoryEdit?>> GetCategoryEdit( int categoryId );
    Task<ServiceReply<bool>> AddBulkCategories( List<CategoryEdit> categories );
    Task<ServiceReply<int>> AddCategory( CategoryEdit dto );
    Task<ServiceReply<bool>> UpdateCategory( CategoryEdit dto );
    Task<ServiceReply<bool>> RemoveCategory( int categoryId );
}