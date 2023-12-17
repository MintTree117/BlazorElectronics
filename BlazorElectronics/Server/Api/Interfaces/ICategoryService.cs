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
    Task<ServiceReply<CategoryEditDtoDto?>> GetCategoryEdit( int categoryId );
    Task<ServiceReply<bool>> AddBulkCategories( List<CategoryEditDtoDto> categories );
    Task<ServiceReply<int>> AddCategory( CategoryEditDtoDto dtoDto );
    Task<ServiceReply<bool>> UpdateCategory( CategoryEditDtoDto dtoDto );
    Task<ServiceReply<bool>> RemoveCategory( int categoryId );
}