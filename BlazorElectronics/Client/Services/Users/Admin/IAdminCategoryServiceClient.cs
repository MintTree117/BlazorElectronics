using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminCategoryServiceClient
{
    Task<ApiReply<CategoriesViewDto?>> GetCategoriesView();
    Task<ApiReply<CategoryEditDto?>> GetCategoryEdit( CategoryGetEditDto data );
    Task<ApiReply<CategoryEditDto?>> AddCategory( CategoryAddDto data );
    Task<ApiReply<bool>> UpdateCategory( CategoryEditDto dto );
    Task<ApiReply<bool>> RemoveCategory( CategoryRemoveDto dto );
}