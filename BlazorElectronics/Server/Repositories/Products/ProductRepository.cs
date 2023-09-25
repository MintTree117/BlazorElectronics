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
    const string PRODUCT_ID_COLUMN = "ProductId";
    const string PRODUCT_NAME_COLUMN = "ProductName";
    const string PRODUCT_RATING_COLUMN = "ProductRating";
    const string VARIANT_ID_COLUMN = "VariantId";
    const string VARIANT_PRICE_MAIN_COLUMN = "VariantPriceMain";
    const string VARIANT_PRICE_SALE_COLUMN = "VariantPriceSale";
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
    
    public async Task<string> TEST_GET_QUERY_STRING( ValidatedSearchFilters searchFilters )
    {
        var productDictionary = new Dictionary<int, Product>();
        var paramDictionary = new Dictionary<string, object>();
        var productQuery = new StringBuilder();
        var countQuery = new StringBuilder();

        await BuildProductSearchQuery( searchFilters, paramDictionary, productQuery, countQuery );
        return productQuery.ToString();
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
    public async Task<IEnumerable<Product>> SearchProducts( ValidatedSearchFilters searchFilters )
    {
        var productDictionary = new Dictionary<int, Product>();
        var paramDictionary = new Dictionary<string, object>();
        var productQuery = new StringBuilder();
        var countQuery = new StringBuilder();

        await BuildProductSearchQuery( searchFilters, paramDictionary, productQuery, countQuery );

        var sqlParamsDynamic = new DynamicParameters( paramDictionary );

        await using SqlConnection connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        Task searchTask = SearchProducts( connection, productQuery.ToString(), productDictionary, sqlParamsDynamic );
        await Task.WhenAll( searchTask );

        return productDictionary.Values;
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
    
    async Task BuildProductSearchQuery( 
        ValidatedSearchFilters filters, Dictionary<string, object> paramDictionary, StringBuilder productQuery, StringBuilder countQuery )
    {
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

            // CATEGORY
            if ( filters.Category != null )
                sharedSQL.Append( $" AND p.{CATEGORY_ID_COLUMN} = @categoryId" );

            // SEARCH TEXT
            if ( !string.IsNullOrEmpty( filters.SearchText ) ) {
                sharedSQL.Append( $@" AND p.{PRODUCT_NAME_COLUMN} LIKE @searchText" );
                paramDictionary.Add( "@searchText", $"%{filters.SearchText}%" );
            }

            // RATING
            if ( filters.MinRating != null ) {
                sharedSQL.Append( $@" AND p.{PRODUCT_RATING_COLUMN} >= @minRating" );
                paramDictionary.Add( "@minRating", filters.MinRating );
            }
            if ( filters.MaxRating != null ) {
                sharedSQL.Append( $@" AND p.{PRODUCT_RATING_COLUMN} <= @maxRating" );
                paramDictionary.Add( "@maxRating", filters.MaxRating );
            }
            
            // VARIANT PRICE SUBQUERY
            sharedSQL.Append( $" AND p.{PRODUCT_ID_COLUMN} IN (" );
            sharedSQL.Append( $" SELECT DISTINCT pvQUERY.{PRODUCT_ID_COLUMN}" );
            sharedSQL.Append( $" FROM {PRODUCT_VARIANTS_TABLE} pvQUERY" );
            sharedSQL.Append( $" WHERE 1=1" );
            if ( filters.MinPrice != null ) {
                sharedSQL.Append( $@" AND pvQUERY.{VARIANT_PRICE_MAIN_COLUMN} >= @minPrice" );
                sharedSQL.Append( $@" OR pvQUERY.{VARIANT_PRICE_SALE_COLUMN} >= @minPrice" );
                paramDictionary.Add( "@minPrice", filters.MinPrice );
            }
            if ( filters.MaxPrice != null ) {
                sharedSQL.Append( $@" AND pvQUERY.{VARIANT_PRICE_MAIN_COLUMN} <= @maxPrice" );
                sharedSQL.Append( $@" OR pvQUERY.{VARIANT_PRICE_SALE_COLUMN} <= @maxPrice" );
                paramDictionary.Add( "@maxPrice", filters.MaxPrice );
            }
            sharedSQL.Append( $" )" );

            // LOOKUP SPECS SUBQUERY
            if ( filters.LookupSpecFilters.Count > 0 ) {
                sharedSQL.Append( $" AND EXISTS (SELECT 1 FROM {PRODUCT_SPECS_LOOKUP_TABLE} psl" );
                sharedSQL.Append( $" WHERE p.{PRODUCT_ID_COLUMN} = psl.{PRODUCT_ID_COLUMN}" );
                for ( int i = 0; i < filters.LookupSpecFilters.Count; i++ ) {
                    var paramName = "@LookupSpecFilter" + i;
                    sharedSQL.Append( $" AND psl.{filters.LookupSpecFilters[ i ].SpecName} = {paramName}" );
                    paramDictionary.Add( paramName, filters.LookupSpecFilters[ i ].SpecValue );
                }
                sharedSQL.Append( ")" );   
            }

            // RAW SPECS SUBQUERY
            if ( filters.RawSpecFilters.Count > 0 ) {
                sharedSQL.Append( $" AND EXISTS (SELECT 1 FROM {PRODUCT_SPECS_RAW_TABLE} psr" );
                sharedSQL.Append( $" WHERE p.{PRODUCT_ID_COLUMN} = psr.{PRODUCT_ID_COLUMN}" );
                for ( int i = 0; i < filters.RawSpecFilters.Count; i++ ) {
                    var paramName = "@RawSpecFilter" + i;
                    sharedSQL.Append( $" AND psr.{filters.RawSpecFilters[ i ].SpecName} = {paramName}" );
                    paramDictionary.Add( paramName, filters.RawSpecFilters[ i ].SpecValue );
                }
                sharedSQL.Append( ")" );   
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
    async Task SearchProducts( SqlConnection connection, string dynamicQuery, Dictionary<int, Product> productDictionary, DynamicParameters sqlParamsDynamic )
    {
        await connection.QueryAsync<Product, ProductVariant, Product>
        ( dynamicQuery, ( product, variant ) => {
                if ( !productDictionary.TryGetValue( product.ProductId, out Product? productEntry ) ) {
                    productEntry = product;
                    productEntry.ProductVariants = new List<ProductVariant>();
                    productDictionary.Add( productEntry.ProductId, productEntry );
                }
                if ( variant != null )
                    productEntry.ProductVariants.Add( variant );
                return productEntry;
            }, 
            sqlParamsDynamic,
            splitOn: $"{PRODUCT_ID_COLUMN},{VARIANT_ID_COLUMN}", 
            commandType: CommandType.Text );
    }
}