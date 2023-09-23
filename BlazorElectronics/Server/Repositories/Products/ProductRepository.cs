using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Shared.DataTransferObjects.Products;
using Microsoft.Extensions.Primitives;

namespace BlazorElectronics.Server.Repositories.Products;

public sealed class ProductRepository : IProductRepository
{
    async Task BuildProductSearchQuery( ValidatedSearchFilters filters, Dictionary<string, object> paramDictionary, StringBuilder productQuery, StringBuilder countQuery )
    {
        
        // WE STILL HAVE TO FIX PRICE
        await Task.Run( () => {
            // BASE
            productQuery.Append( $"SELECT p.*, pv.*" );
            countQuery.Append( $"SELECT COUNT(*)" );

            // SHARED
            var sharedSQL = new StringBuilder();
            sharedSQL.Append( $" FROM {PRODUCTS_TABLE} p" );
            sharedSQL.Append( $" LEFT JOIN {PRODUCT_VARIANTS_TABLE} pv" );
            sharedSQL.Append( $" ON p.{PRODUCT_ID_COLUMN} = pv.{PRODUCT_ID_COLUMN}" );
            sharedSQL.Append( $" WHERE 1=1" );

            // MAIN FILTERS
            if ( filters.Category != null )
                sharedSQL.Append( $" AND p.{CATEGORY_ID_COLUMN} = @categoryId" );
            if ( filters.MinPrice != null ) {
                sharedSQL.Append( @" AND p.Price >= @minPrice" );
                paramDictionary.Add( "@minPrice", filters.MinPrice );
            }
            if ( filters.MaxPrice != null ) {
                sharedSQL.Append( @" AND Price <= @maxPrice" );
                paramDictionary.Add( "@maxPrice", filters.MaxPrice );
            }
            if ( filters.MinRating != null ) {
                sharedSQL.Append( $@" AND p.{PRODUCT_RATING_COLUMN} >= @minRating" );
                paramDictionary.Add( "@minRating", filters.MinRating );
            }

            // SEARCH TEXT
            if ( !string.IsNullOrEmpty( filters.SearchText ) ) {
                sharedSQL.Append( $@" AND p.{PRODUCT_NAME_COLUMN} LIKE @searchText" );
                paramDictionary.Add( "@searchText", $"%{filters.SearchText}%" );
            }
            
            // LOOKUP SPECS
            sharedSQL.Append( $" AND EXISTS (SELECT 1 FROM {PRODUCT_SPECS_LOOKUP_TABLE} psl" );
            sharedSQL.Append( $" WHERE p.{PRODUCT_ID_COLUMN} = psl.{PRODUCT_ID_COLUMN}" );
            for ( int i = 0; i < filters.LookupSpecFilters.Count; i++ ) {
                var paramName = "@LookupSpecFilter" + i;
                sharedSQL.Append( $" AND psl.{filters.LookupSpecFilters[ i ].SpecName} = {paramName}" );
                paramDictionary.Add( paramName, filters.LookupSpecFilters[ i ].SpecValue );
            }

            // RAW SPECS
            sharedSQL.Append( $" AND EXISTS (SELECT 1 FROM {PRODUCT_SPECS_RAW_TABLE} psr" );
            sharedSQL.Append( $" WHERE p.{PRODUCT_ID_COLUMN} = psr.{PRODUCT_ID_COLUMN}" );
            for ( int i = 0; i < filters.RawSpecFilters.Count; i++ ) {
                var paramName = "@RawSpecFilter" + i;
                sharedSQL.Append( $" AND psr.{filters.RawSpecFilters[ i ].SpecName} = {paramName}" );
                paramDictionary.Add( paramName, filters.RawSpecFilters[ i ].SpecValue );
            }

            // ROW COUNT
            productQuery.Append( sharedSQL );
            productQuery.Append( $@" ORDER BY p.{PRODUCT_ID_COLUMN} OFFSET @queryOffset ROWS FETCH NEXT @queryRows ROWS ONLY;" );
            countQuery.Append( sharedSQL );
            countQuery.Append( ";" );
            paramDictionary.Add( "@queryOffset", filters.Page * filters.Rows );
            paramDictionary.Add( "@queryRows", filters.Rows );
        } );
    }
    
    
    
    const string PRODUCT_ID_COLUMN = "ProductId";
    const string PRODUCT_NAME_COLUMN = "ProductName";
    const string PRODUCT_RATING_COLUMN = "ProductRating";
    const string CATEGORY_ID_COLUMN = "CategoryId";
    
    const string PRODUCTS_TABLE = "Products";
    const string PRODUCT_FEATURES_TABLE = "ProductFeatures";
    const string PRODUCT_CATEGORIES_TABLE = "ProductCategories";
    const string PRODUCT_DESCRIPTIONS_TABLE = "ProductDescriptions";
    const string PRODUCT_IMAGES_TABLE = "ProductImages";
    const string PRODUCT_REVIEWS_TABLE = "ProductReviews";
    const string PRODUCT_SPECS_LOOKUP_TABLE = "ProductSpecsLookup";
    const string PRODUCT_SPECS_RAW_TABLE = "ProductSpecsRaw";
    const string PRODUCT_VARIANTS_TABLE = "ProductVariants";

    const string STORED_PROCEDURE_GET_PRODUCTS = "Get_Products";
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
    public async Task<IEnumerable<Product>> GetAllProducts()
    {
        await using SqlConnection connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        var productDictionary = new Dictionary<int, Product>();
        
        await connection.QueryAsync
            <Product, ProductVariant, Product?>
            ( STORED_PROCEDURE_GET_PRODUCTS,
                ( product, variant ) => {
                    if ( product == null )
                        return null;
                    productDictionary.TryAdd( product.ProductId, product );
                    if ( variant != null )
                        product.ProductVariants.Add( variant );
                    return product;
                },
                splitOn: PRODUCT_ID_COLUMN,
                commandType: CommandType.StoredProcedure );

        return productDictionary.Values;
    }
    public async Task<IEnumerable<Product>> SearchProducts( ProductSearchFilters_DTO searchFilters )
    {
        var productDictionary = new Dictionary<int, Product>();
        var paramDictionary = new Dictionary<string, object>();

        var productQuery = new StringBuilder();
        var countQuery = new StringBuilder();

        return null;
    }
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