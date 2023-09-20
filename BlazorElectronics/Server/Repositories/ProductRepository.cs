using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Shared.DataTransferObjects.Products;
using Microsoft.Data.SqlClient;
using Dapper;

namespace BlazorElectronics.Server.Repositories;

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
    public async Task<ProductDetails> GetProductDetails()
    {
        await using SqlConnection connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        const string query = $"SELECT p.*, pv.*, pd.*, pi.*, pr.*, psl.*, psr.* " +
                             $"FROM {PRODUCTS_TABLE} p " +
                             $"LEFT JOIN {PRODUCT_DESCRIPTIONS_TABLE} pd ON p.ProductId = pd.ProductId " +
                             $"LEFT JOIN {PRODUCT_VARIANTS_TABLE} pv ON p.ProductId = pv.ProductId " +
                             $"LEFT JOIN {PRODUCT_IMAGES_TABLE} pi ON p.ProductId = pi.ProductId " +
                             $"LEFT JOIN {PRODUCT_REVIEWS_TABLE} pr ON p.ProductId = pr.ProductId "; //+
                             //$"LEFT JOIN {PRODUCT_SPECS_LOOKUP_TABLE} psl ON p.ProductId = psl.ProductId " +
                             //$"LEFT JOIN {PRODUCT_SPECS_RAW_TABLE} psr ON p.ProductId = psr.ProductId ";

        var productDictionary = new Dictionary<int, Product>();
        var productDetails = new ProductDetails();

        await connection.QueryAsync<Product, ProductDescription, ProductVariant, ProductImage, ProductReview, ProductDetails>
        ( query, ( product, description, variant, image, review ) => {
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
            /*if ( !productDictionary.TryGetValue( product.ProductId, out Product? productEntry ) ) {
                productEntry = product;
                productEntry.ProductVariants = new List<ProductVariant>();
                productDictionary.Add( productEntry.ProductId, productEntry );
            }
            if ( variant != null )
                productEntry.ProductVariants!.Add( variant );
            return productEntry;*/
        }, splitOn: $"{PRODUCT_ID_COLUMN}" );

        return productDetails;

        //return await connection.QuerySingleAsync<ProductDetails_DTO>( query );
    }
}