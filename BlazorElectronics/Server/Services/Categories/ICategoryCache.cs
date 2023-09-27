using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryCache
{
    Task<Categories_DTO?> Get();
    Task Set( Categories_DTO dto );
}