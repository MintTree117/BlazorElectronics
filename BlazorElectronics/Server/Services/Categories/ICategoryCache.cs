using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryCache
{
    Task<Models.Categories.CategoryMeta?> Get();
    Task Set( Models.Categories.CategoryMeta dto );
}