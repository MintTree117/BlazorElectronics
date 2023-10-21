using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Features;
using BlazorElectronics.Server.Models.Products;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Features;

public class FeaturedDealsRepository : DapperRepository<FeaturedDeal>, IFeaturedDealsRepository
{
    const string STORED_PROCEDURE_GET_FEATURED_DEALS = "Get_FeaturedDeals";
    const string STORED_PROCEDURE_GET_FEATURED_DEAL_BY_ID = "Get_FeaturedDealById";
    
    public FeaturedDealsRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public override async Task<IEnumerable<FeaturedDeal>?> GetAll()
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();

        var dealDictionary = new Dictionary<int, FeaturedDeal>();

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
    public override async Task<FeaturedDeal?> GetById( int id )
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        return await connection.QueryFirstAsync<FeaturedDeal>( STORED_PROCEDURE_GET_FEATURED_DEAL_BY_ID, commandType: CommandType.StoredProcedure );
    }
}