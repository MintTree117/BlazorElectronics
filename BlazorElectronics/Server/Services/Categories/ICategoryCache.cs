using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Shared.Outbound.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryCache
{
    Task<List<string>?> GetPrimaryDescriptions();
    Task<CategoriesResponse?> GetCategoriesResponse();
    Task<CategoryUrlMap?> GetUrlMap();

    Task SetPrimaryDescriptions( List<string> descriptions );
    Task SetCategoriesResponse( CategoriesResponse response );
    Task SetUrlMap( CategoryUrlMap map );
}