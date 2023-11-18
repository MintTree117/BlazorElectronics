using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Client.Services.Admin;

public interface IAdminCategoryServiceClient
{
    Task<bool> AddCategory( AddUpdateCategoryDto dto );
    Task<bool> UpdateCategory( AddUpdateCategoryDto dto );
    Task<bool> DeleteCategory( DeleteCategoryDto dto );
}