using BlazorElectronics.Server.Dtos.Categories;
using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Shared.Outbound.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryCache
{
    Task<List<string>?> GetPrimaryDescriptions();
    Task<CategoriesDto?> GetCategoriesResponse();
    Task<CategoryUrlMap?> GetUrlMap();

    Task SetPrimaryDescriptions( List<string> descriptions );
    Task SetCategoriesResponse( CategoriesDto dto );
    Task SetUrlMap( CategoryUrlMap map );
}