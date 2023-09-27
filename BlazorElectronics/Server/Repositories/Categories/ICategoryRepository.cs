using BlazorElectronics.Server.Models.Categories;

namespace BlazorElectronics.Server.Repositories.Categories;

public interface ICategoryRepository
{
    Task<Dictionary<string, Category>?> GetCategories2();
    Task<List<Category>?> GetCategories();
}