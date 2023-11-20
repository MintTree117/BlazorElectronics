using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Client.Services.Admin;

public interface IAdminCategoryServiceClient
{
    Task<ApiReply<CategoryViewDto?>> GetCategoriesView();
    Task<ApiReply<EditCategoryDto?>> GetCategoryEdit( GetCategoryEditDto request );
    Task<ApiReply<EditCategoryDto?>> AddCategory( AddCategoryDto dto );
    Task<ApiReply<bool>> UpdateCategory( EditCategoryDto dto );
    Task<ApiReply<bool>> RemoveCategory( RemoveCategoryDto dto );
}