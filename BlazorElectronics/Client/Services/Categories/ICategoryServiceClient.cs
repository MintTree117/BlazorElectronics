using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public interface ICategoryServiceClient
{
    Task<ServiceResponse<Categories_DTO?>> GetCategories();
}