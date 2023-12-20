using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface ICategoryService
{
    // USER
    Task<ServiceReply<List<int>?>> GetPrimaryCategoryIds(); 
    Task<ServiceReply<CategoryData?>> GetCategoryData();
    Task<ServiceReply<List<CategoryLightDto>?>> GetCategoryResponse();
    Task<ServiceReply<int>> ValidateCategoryUrl( string url );

    // ADMIN
    Task<ServiceReply<List<CategoryViewDtoDto>?>> GetCategoriesView();
    Task<ServiceReply<CategoryEditDto?>> GetCategoryEdit( int categoryId );
    Task<ServiceReply<bool>> AddBulkCategories( List<CategoryEditDto> categories );
    Task<ServiceReply<int>> AddCategory( CategoryEditDto dto );
    Task<ServiceReply<bool>> UpdateCategory( CategoryEditDto dto );
    Task<ServiceReply<bool>> RemoveCategory( int categoryId );
}