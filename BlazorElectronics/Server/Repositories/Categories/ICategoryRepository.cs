using BlazorElectronics.Server.Models.Categories;

namespace BlazorElectronics.Server.Repositories.Categories;

public interface ICategoryRepository
{
    Task<List<Category>> GetCategories();
}