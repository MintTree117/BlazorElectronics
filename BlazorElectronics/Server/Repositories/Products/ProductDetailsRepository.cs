using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductDetailsRepository : DapperRepository<ProductDetails>, IProductDetailsRepository
{
    const string STORED_PROCEDURE_GET_PRODUCT_DETAILS = "Get_ProductDetails";
    
    public ProductDetailsRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public override async Task<IEnumerable<ProductDetails>?> GetAll()
    {
        IEnumerable<ProductDetails>? nullResponse = new List<ProductDetails>( 0 );

        await Task.Run( () => { nullResponse = new List<ProductDetails>( 0 ); } );

        return nullResponse;
    }
    public override async Task<ProductDetails?> GetById( int id )
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();

        var productDetails = new ProductDetails();
        var parameters = new DynamicParameters( new { Id = id } );
        const string splitOnColumns = $"{SqlConsts.COLUMN_PRODUCT_ID},{SqlConsts.COLUMN_PRODUCT_DESCRIPTION_ID_COLUMN},{SqlConsts.COLUMN_VARIANT_ID_PRIMARY},{SqlConsts.COLUMN_PRODUCT_IMAGE_ID},{SqlConsts.COLUMN_PRODUCT_REVIEW_ID}";

        await connection.QueryAsync<Product, ProductDescription, ProductVariant, ProductImage, ProductReview, ProductDetails>
        ( STORED_PROCEDURE_GET_PRODUCT_DETAILS, ( product, description, variant, image, review ) =>
            {
                productDetails.Product ??= product;
                
                if ( description != null )
                    productDetails.ProductDescription = description;
                if ( variant != null )
                    productDetails.Product.ProductVariants.Add( variant );
                if ( image != null )
                    productDetails.ProductImages.Add( image );
                if ( review != null )
                    productDetails.ProductReviews.Add( review );
                
                return productDetails;
            },
            parameters,
            splitOn: splitOnColumns,
            commandType: CommandType.StoredProcedure );

        return productDetails;
    }
}