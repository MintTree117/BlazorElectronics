using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Features;
using BlazorElectronics.Server.Models.Products;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Features;

public class FeaturesRepository : DapperRepository, IFeaturesRepository
{
    const string STORED_PROCEDURE_GET_FEATURED_PRODUCTS = "Get_FeaturedProducts";
    const string STORED_PROCEDURE_GET_FEATURED_DEALS = "Get_FeaturedDeals";

    public FeaturesRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<IEnumerable<FeaturedProduct>?> GetFeaturedProducts()
    {
        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            return await connection.QueryAsync<FeaturedProduct>( STORED_PROCEDURE_GET_FEATURED_PRODUCTS, commandType: CommandType.StoredProcedure );
        }
        catch ( SqlException e )
        {
            throw new RepositoryException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new RepositoryException( e.Message, e );
        }
    }
    public async Task<IEnumerable<FeaturedDeal>?> GetFeaturedDeals()
    {
        var dealDictionary = new Dictionary<int, FeaturedDeal>();

        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            await connection.QueryAsync<Product, ProductVariant, Product>
            ( STORED_PROCEDURE_GET_FEATURED_DEALS, ( product, variant ) =>
                {
                    if ( !dealDictionary.TryGetValue( product.ProductId, out FeaturedDeal? dealEntry ) )
                    {
                        dealEntry = new FeaturedDeal {
                            ProductId = product.ProductId,
                            ProductTitle = product.ProductTitle,
                            ProductThumbnail = product.ProductThumbnail,
                            ProductRating = product.ProductRating
                        };
                        dealDictionary.Add( product.ProductId, dealEntry );
                    }
                    if ( variant == null )
                        return product;
                    dealEntry.VariantId = variant.VariantId;
                    dealEntry.OriginalPrice = variant.VariantPriceMain;
                    dealEntry.SalePrice = variant.VariantPriceSale;
                    return product;
                },
                splitOn: SqlConsts.COLUMN_PRODUCT_ID,
                commandType: CommandType.StoredProcedure );

            return dealDictionary.Values;
        }
        catch ( SqlException e )
        {
            throw new RepositoryException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new RepositoryException( e.Message, e );
        }
    }
}