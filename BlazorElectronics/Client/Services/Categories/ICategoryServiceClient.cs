using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public interface ICategoryServiceClient
{
    Task<ApiReply<CategoriesResponse?>> GetCategories();
}