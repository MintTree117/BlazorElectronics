using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminCategoryServiceClient
{
    Task<ServiceReply<CategoriesViewDto?>> GetCategoriesView();
    Task<ServiceReply<CategoryEditDto?>> GetCategoryEdit( CategoryGetEditDto data );
    Task<ServiceReply<CategoryEditDto?>> AddCategory( CategoryAddDto data );
    Task<ServiceReply<bool>> UpdateCategory( CategoryEditDto dto );
    Task<ServiceReply<bool>> RemoveCategory( CategoryRemoveDto dto );
}