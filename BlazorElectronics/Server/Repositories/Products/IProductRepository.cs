using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductRepository : IDapperRepository<Product>
{
    Task<string> TEST_GET_QUERY_STRING( int categoryId, ValidatedSearchFilters searchFilters );
    Task<(IEnumerable<Product>?, int)?> SearchProducts( int categoryId, ValidatedSearchFilters searchFilters );
}