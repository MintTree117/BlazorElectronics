using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductDetailsRepository : DapperRepository, IProductDetailsRepository
{
    const string STORED_PROCEDURE_GET_PRODUCT_DETAILS = "Get_ProductDetails";
    const string QUERY_PARAM_PRODUCT_ID = "@ProductId";

    public ProductDetailsRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<ProductDetails?> GetProductDetailsById( int productId )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_PRODUCT_ID, productId );

        return await TryQueryAsync( GetProductDetailsQuery, dynamicParams );
    }

    static async Task<ProductDetails?> GetProductDetailsQuery( SqlConnection connection, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? result = await connection.QueryMultipleAsync( STORED_PROCEDURE_GET_PRODUCT_DETAILS, dynamicParams, commandType: CommandType.StoredProcedure ).ConfigureAwait( false );

        var productDetails = new ProductDetails();
        
        productDetails.Product = result.Read<Product>().FirstOrDefault();
        productDetails.Product.ProductVariants = result.Read<ProductVariant>().ToList();
        productDetails.ProductDescription = result.Read<ProductDescription>().FirstOrDefault();
        productDetails.ProductImages = result.Read<ProductImage>().ToList();
        productDetails.ProductReviews = result.Read<ProductReview>().ToList();
        
        return productDetails;
    }
}