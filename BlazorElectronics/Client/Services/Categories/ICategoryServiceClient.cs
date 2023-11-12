using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public interface ICategoryServiceClient
{
    Task<ApiReply<CategoriesResponse?>> GetCategories();
}