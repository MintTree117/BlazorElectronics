using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductDetailsRepository : DapperRepository<ProductDetails>, IProductDetailsRepository
{
    const string PRODUCT_ID_COLUMN = "ProductId";
    const string DESCRIPTION_ID_COLUMN = "DescriptionId";
    const string VARIANT_ID_COLUMN = "VariantId";
    const string IMAGE_ID_COLUMN = "ImageId";
    const string REVIEW_ID_COLUMN = "ReviewId";
    
    const string STORED_PROCEDURE_GET_PRODUCT_DETAILS = "Get_ProductDetails";
    
    public ProductDetailsRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public override async Task<IEnumerable<ProductDetails>?> GetAll()
    {
        return null;
    }
    public override async Task<ProductDetails?> GetById( int id )
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();

        var productDetails = new ProductDetails();
        var parameters = new DynamicParameters( new { Id = id } );
        const string splitOnColumns = $"{PRODUCT_ID_COLUMN},{DESCRIPTION_ID_COLUMN},{VARIANT_ID_COLUMN},{IMAGE_ID_COLUMN},{REVIEW_ID_COLUMN}";

        await connection.QueryAsync<Product, ProductDescription, ProductVariant, ProductImage, ProductReview, ProductDetails>
        ( STORED_PROCEDURE_GET_PRODUCT_DETAILS, ( product, description, variant, image, review ) =>
            {
                if ( productDetails.Product == null )
                    productDetails.Product = product;
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