using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminCategoryServiceClient
{
    Task<ApiReply<CategoryViewDto?>> GetCategoriesView();
    Task<ApiReply<EditCategoryDto?>> GetCategoryEdit( GetCategoryEditDto data );
    Task<ApiReply<EditCategoryDto?>> AddCategory( AddCategoryDto data );
    Task<ApiReply<bool>> UpdateCategory( EditCategoryDto dto );
    Task<ApiReply<bool>> RemoveCategory( RemoveCategoryDto dto );
}