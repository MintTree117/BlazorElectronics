using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Models.Products.Details;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductDetailsRepository : DapperRepository, IProductDetailsRepository
{
    const string PROCEDURE_GET_PRODUCT_DETAILS = "Get_ProductDetails";

    public ProductDetailsRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<ProductDetailsModel?> GetProductDetails( int productId )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( PARAM_PRODUCT_ID, productId );

        return await TryQueryAsync( GetProductOverviewQuery, dynamicParams );
    }
    static async Task<ProductDetailsModel?> GetProductOverviewQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET_PRODUCT_DETAILS, dynamicParams, commandType: CommandType.StoredProcedure ).ConfigureAwait( false );
        
        var productDetails = new ProductDetailsModel
        {
            PrimaryCategory = multi.ReadFirstOrDefault<int>(),
            SecondaryCategories = multi.ReadFirstOrDefault<string>(),
            TertiaryCategories = multi.ReadFirstOrDefault<string>(),
            Overview = multi.ReadFirstOrDefault<ProductOverviewModel>(),
            ProductDescription = multi.ReadFirstOrDefault<string>(),
            Images = multi.Read<ProductImageModel>().ToList(),
            Variants = multi.Read<ProductVariantModel>().ToList()
        };

        return productDetails;
    }
}