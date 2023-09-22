using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Shared.DataTransferObjects.Products;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Products;

public sealed class ProductRepository : IProductRepository
{
    const string PRODUCT_ID_COLUMN = "ProductId";
    
    const string PRODUCTS_TABLE = "Products";
    const string PRODUCT_FEATURES_TABLE = "ProductFeatures";
    const string PRODUCT_CATEGORIES_TABLE = "ProductCategories";
    const string PRODUCT_DESCRIPTIONS_TABLE = "ProductDescriptions";
    const string PRODUCT_IMAGES_TABLE = "ProductImages";
    const string PRODUCT_REVIEWS_TABLE = "ProductReviews";
    const string PRODUCT_SPECS_LOOKUP_TABLE = "ProductSpecsLookup";
    const string PRODUCT_SPECS_RAW_TABLE = "ProductSpecsRaw";
    const string PRODUCT_VARIANTS_TABLE = "ProductVariants";

    const string STORED_PROCEDURE_GET_PRODUCT_DETAILS = "Get_ProductDetails";

    readonly DapperContext _dbContext;

    public ProductRepository( DapperContext dapperContext )
    {
        _dbContext = dapperContext;
    }

    public async Task<IEnumerable<Product>?> GetProducts()
    {
        await using SqlConnection connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        const string query = $"SELECT * FROM {PRODUCTS_TABLE}";
        
        return await connection.QueryAsync<Product>( query );
    }
    public Task<IEnumerable<Product>> GetAllProducts() { throw new NotImplementedException(); }
    public Task<IEnumerable<Product>> SearchProducts( ProductSearchFilters_DTO searchFilters ) { throw new NotImplementedException(); }
    public async Task<ProductDetails> GetProductDetails( int productId )
    {
        await using SqlConnection connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        var productDetails = new ProductDetails();
        var parameters = new DynamicParameters( new { id = productId } );

        await connection.QueryAsync
            <Product, ProductDescription, ProductVariant, ProductImage, ProductReview, ProductDetails>
            ( STORED_PROCEDURE_GET_PRODUCT_DETAILS,
            ( product, description, variant, image, review ) => {
                productDetails.Product ??= product;
                if ( description != null )
                    productDetails.ProductDescription = description;
                if ( variant != null )
                    productDetails.ProductVariants.Add( variant );
                if ( image != null )
                    productDetails.ProductImages.Add( image );
                if ( review != null )
                    productDetails.ProductReviews.Add( review );
                return productDetails;
            },
            parameters,
            splitOn: PRODUCT_ID_COLUMN,
            commandType: CommandType.StoredProcedure );

        return productDetails;
    }
}