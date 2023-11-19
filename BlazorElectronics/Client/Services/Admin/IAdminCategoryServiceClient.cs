using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Client.Services.Admin;

public interface IAdminCategoryServiceClient
{
    Task<ApiReply<AddUpdateCategoryDto?>> GetCategoryEdit( GetCategoryEditRequest dto );
    Task<ApiReply<AddUpdateCategoryDto?>> AddCategory( AddUpdateCategoryDto dto );
    Task<ApiReply<bool>> UpdateCategory( AddUpdateCategoryDto dto );
    Task<ApiReply<bool>> RemoveCategory( DeleteCategoryDto dto );
}