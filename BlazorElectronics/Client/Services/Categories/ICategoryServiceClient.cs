using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public interface ICategoryServiceClient
{
    Task<Categories_DTO?> GetCategories();
}