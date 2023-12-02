using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminCategoryServiceClient : IAdminViewService<CategoryViewDto>
{
    Task<ServiceReply<CategoryEditDto?>> GetCategoryEdit( IntDto data );
    Task<ServiceReply<CategoryEditDto?>> AddCategory( CategoryEditDto data );
    Task<ServiceReply<bool>> UpdateCategory( CategoryEditDto dto );
}