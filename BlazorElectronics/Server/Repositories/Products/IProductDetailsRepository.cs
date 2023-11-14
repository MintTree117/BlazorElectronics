using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Models.Products.Specs;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductDetailsRepository
{
    Task<ProductOverviewModel?> GetProductOverview( int productId );
    Task<ProductSpecsModel?> GetProductSpecs( int productId, int primaryCategoryId, CachedSpecData specData );
}