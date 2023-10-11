using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public sealed class ProductRepository : DapperRepository<Product>, IProductRepository
{
    const string COLUMN_PRODUCT_ID = "ProductId";
    const string COLUMN_PRODUCT_NAME = "ProductName";
    const string COLUMN_PRODUCT_RATING = "ProductRating";
    const string COLUMN_PRODUCT_THUMBNAIL = "ProductThumbnail";
    
    const string COLUMN_PRODUCT_DESCRIPTION = "Description";
    
    const string COLUMN_VARIANT_ID = "VariantId";
    const string COLUMN_VARIANT_NAME = "VariantName";
    const string COLUMN_VARIANT_PRICE_MAIN = "VariantPriceMain";
    const string COLUMN_VARIANT_PRICE_SALE = "VariantPriceSale";
    
    const string COLUMN_CATEGORY_ID = "CategoryId";
    
    const string TABLE_PRODUCTS = "Products";
    const string PRODUCT_FEATURES_TABLE = "ProductFeatures";
    const string TABLE_PRODUCT_CATEGORIES = "ProductCategories";
    const string TABLE_PRODUCT_DESCRIPTIONS = "ProductDescriptions";
    const string PRODUCT_IMAGES_TABLE = "ProductImages";
    const string PRODUCT_REVIEWS_TABLE = "ProductReviews";
    const string PRODUCT_SPECS_LOOKUP_TABLE = "ProductSpecsLookup";
    const string PRODUCT_SPECS_RAW_TABLE = "ProductSpecsRaw";
    const string TABLE_PRODUCT_VARIANTS = "ProductVariants";

    const string QUERY_PARAM_CATEGORY_ID = "@categoryId";
    const string QUERY_PARAM_SEARCH_TEXT = "@searchText";
    const string QUERY_PARAM_MIN_RATING = "@minRating";
    const string QUERY_PARAM_MAX_RATING = "@maxRating";
    const string QUERY_PARAM_MIN_PRICE = "@minPrice";
    const string QUERY_PARAM_MAX_PRICE = "@maxPrice";
    const string QUERY_PARAM_LOOKUP_SPEC_FILTER = "@lookupSpecFilter";
    const string QUERY_PARAM_RAW_SPEC_FILTER = "@lookupRawSpecFilter";
    const string QUERY_PARAM_QUERY_OFFSET = "@queryOffset";
    const string QUERY_PARAM_QUERY_ROWS = "@queryRows";

    const string STORED_PROCEDURE_COUNT_PRODUCTS_ALL = "Count_Products";
    const string STORED_PROCEDURE_COUNT_PRODUCTS_SEARCH = "Count_ProductsSearch";
    const string STORED_PROCEDURE_GET_NAMES_BY_SEARCH_TEXT = "Get_ProductNamesBySearchText";
    const string STORED_PROCEDURE_GET_PRODUCTS = "Get_Products";

    public ProductRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<string> TEST_GET_QUERY_STRING( ValidatedSearchFilters searchFilters )
    {
        var productQuery = new StringBuilder();
        var countQuery = new StringBuilder();

        await BuildProductSearchQuery( searchFilters, productQuery, countQuery );
        return productQuery.ToString() + searchFilters.CategoryId.Value;
    }
    
    public override async Task<IEnumerable<Product>?> GetAll()
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        
        var productDictionary = new Dictionary<int, Product>();

        return await connection.QueryAsync<Product, ProductVariant, Product>
        ( STORED_PROCEDURE_GET_PRODUCTS, ( product, variant ) =>
            {
                if ( !productDictionary.TryGetValue( product.ProductId, out Product? productEntry ) )
                {
                    productEntry = product;
                    productDictionary.Add( productEntry.ProductId, productEntry );
                }
                if ( variant != null && !product.ProductVariants.Contains( variant ) )
                    product.ProductVariants.Add( variant );
                return product;
            },
            splitOn: COLUMN_PRODUCT_ID,
            commandType: CommandType.StoredProcedure );
    }
    public override Task<Product?> GetById( int id ) { throw new NotImplementedException(); }
    public async Task<IEnumerable<string>?> GetSearchSuggestions( string searchText )
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        var searchParam = new { SearchText = searchText };
        return await connection.QueryAsync<string>( 
            STORED_PROCEDURE_GET_NAMES_BY_SEARCH_TEXT, searchParam, commandType: CommandType.StoredProcedure );
    }
    public async Task<ProductSearch?> SearchProducts( ValidatedSearchFilters searchFilters )
    {
        var productQuery = new StringBuilder();
        var countQuery = new StringBuilder();

        DynamicParameters dynamicParams = await BuildProductSearchQuery( searchFilters, productQuery, countQuery );

        await using SqlConnection connection = await _dbContext.GetOpenConnection();

        Task<IEnumerable<Product>?> productTask = ExecuteSearchProducts( connection, productQuery.ToString(), dynamicParams );
        Task<int> countTask = CountProducts( connection, countQuery.ToString(), dynamicParams );

        await Task.WhenAll( productTask, countTask );

        if ( productTask.Result == null || countTask.Result < 0 )
            return null;

        return new ProductSearch {
            Products = productTask.Result,
            Count = countTask.Result
        };
    }
    
    async Task<DynamicParameters> BuildProductSearchQuery( ValidatedSearchFilters filters, StringBuilder productQuery, StringBuilder countQuery )
    {
        var paramDictionary = new Dictionary<string, object>();

        await Task.Run( () =>
        {
            // BASE
            productQuery.Append( $" SELECT p.*, pv.*" );
            countQuery.Append( $" SELECT COUNT(*)" );

            // SHARED
            var sharedSQL = new StringBuilder();
            sharedSQL.Append( $" FROM {TABLE_PRODUCTS} p" );

            // JOIN VARIANTS
            sharedSQL.Append( $" LEFT JOIN {TABLE_PRODUCT_VARIANTS} pv" );
            sharedSQL.Append( $" ON p.{COLUMN_PRODUCT_ID} = pv.{COLUMN_PRODUCT_ID}" );
            
            // JOIN CATEGORY
            if ( filters.CategoryId != null )
            {
                sharedSQL.Append( $" LEFT JOIN {TABLE_PRODUCT_CATEGORIES} pc" );
                sharedSQL.Append( $" ON p.{COLUMN_PRODUCT_ID} = pc.{COLUMN_PRODUCT_ID}" );    
            }
            // JOIN SEARCH TEXT
            if ( !string.IsNullOrEmpty( filters.SearchText ) )
            {
                sharedSQL.Append( $" LEFT JOIN {TABLE_PRODUCT_DESCRIPTIONS} pd" );
                sharedSQL.Append( $" ON p.{COLUMN_PRODUCT_ID} = pd.{COLUMN_PRODUCT_ID}" );
            }

            // CLAUSES
            sharedSQL.Append( $" WHERE 1=1" );

            // CLAUSE CATEGORY
            if ( filters.CategoryId != null )
            {
                sharedSQL.Append( $@" AND pc.{COLUMN_CATEGORY_ID} = {QUERY_PARAM_CATEGORY_ID}" );
                paramDictionary.Add( QUERY_PARAM_CATEGORY_ID, filters.CategoryId.Value );
            }
            // CLAUSE SEARCH TEXT
            if ( !string.IsNullOrEmpty( filters.SearchText ) )
            {
                sharedSQL.Append( $@" AND (p.{COLUMN_PRODUCT_NAME} LIKE {QUERY_PARAM_SEARCH_TEXT}" );
                sharedSQL.Append( $@" OR pd.{COLUMN_PRODUCT_DESCRIPTION} LIKE {QUERY_PARAM_SEARCH_TEXT})" );
                paramDictionary.Add( QUERY_PARAM_SEARCH_TEXT, $"%{filters.SearchText}%" );
            }

            // CLAUSE RATING
            if ( filters.MinRating != null )
            {
                sharedSQL.Append( $@" AND p.{COLUMN_PRODUCT_RATING} >= {QUERY_PARAM_MIN_RATING}" );
                paramDictionary.Add( QUERY_PARAM_MIN_RATING, filters.MinRating );
            }
            if ( filters.MaxRating != null )
            {
                sharedSQL.Append( $@" AND p.{COLUMN_PRODUCT_RATING} <= {QUERY_PARAM_MAX_RATING}" );
                paramDictionary.Add( QUERY_PARAM_MAX_RATING, filters.MaxRating );
            }
            
            // CLAUSE PRICE
            if ( filters.MinPrice != null || filters.MaxPrice != null )
            {
                sharedSQL.Append( $" AND p.{COLUMN_PRODUCT_ID} IN (" );
                sharedSQL.Append( $" SELECT pvQuery.{COLUMN_PRODUCT_ID}" );
                sharedSQL.Append( $" FROM {TABLE_PRODUCT_VARIANTS} pvQuery" );
                sharedSQL.Append( $" WHERE 1=1" );

                if ( filters.MinPrice != null )
                {
                    sharedSQL.Append( $" AND pvQuery.{COLUMN_VARIANT_PRICE_MAIN} >= {QUERY_PARAM_MIN_PRICE}" );
                    paramDictionary.Add( QUERY_PARAM_MIN_PRICE, filters.MinPrice.Value );
                }

                if ( filters.MaxPrice != null )
                {
                    sharedSQL.Append( $" AND pvQuery.{COLUMN_VARIANT_PRICE_SALE} <= {QUERY_PARAM_MAX_PRICE}" );
                    paramDictionary.Add( QUERY_PARAM_MAX_PRICE, filters.MaxPrice.Value );
                }
                
                sharedSQL.Append( $" )" );
            }

            // CLAUSE LOOKUP SPECS SUBQUERY
            if ( filters.LookupSpecFilters.Count > 0 )
            {
                sharedSQL.Append( $" AND EXISTS (SELECT 1 FROM {PRODUCT_SPECS_LOOKUP_TABLE} psl" );
                sharedSQL.Append( $" WHERE p.{COLUMN_PRODUCT_ID} = psl.{COLUMN_PRODUCT_ID}" );
                for ( int i = 0; i < filters.LookupSpecFilters.Count; i++ )
                {
                    var paramName = QUERY_PARAM_LOOKUP_SPEC_FILTER + i;
                    sharedSQL.Append( $" AND psl.{filters.LookupSpecFilters[ i ].SpecName} = {paramName}" );
                    paramDictionary.Add( paramName, filters.LookupSpecFilters[ i ].SpecValue );
                }
                sharedSQL.Append( ")" );
            }
            
            // CLAUSE RAW SPECS SUBQUERY
            if ( filters.RawSpecFilters.Count > 0 )
            {
                sharedSQL.Append( $" AND EXISTS (SELECT 1 FROM {PRODUCT_SPECS_RAW_TABLE} psr" );
                sharedSQL.Append( $" WHERE p.{COLUMN_PRODUCT_ID} = psr.{COLUMN_PRODUCT_ID}" );
                for ( int i = 0; i < filters.RawSpecFilters.Count; i++ )
                {
                    var paramName = QUERY_PARAM_RAW_SPEC_FILTER + i;
                    sharedSQL.Append( $" AND psr.{filters.RawSpecFilters[ i ].SpecName} = {paramName}" );
                    paramDictionary.Add( paramName, filters.RawSpecFilters[ i ].SpecValue );
                }
                sharedSQL.Append( ")" );
            }

            // ORDER BY AND ROW COUNT
            productQuery.Append( sharedSQL );
            productQuery.Append( $@" ORDER BY p.{COLUMN_PRODUCT_ID} OFFSET {QUERY_PARAM_QUERY_OFFSET} ROWS FETCH NEXT {QUERY_PARAM_QUERY_ROWS} ROWS ONLY;" );
            countQuery.Append( sharedSQL );
            countQuery.Append( ";" );
            paramDictionary.Add( QUERY_PARAM_QUERY_OFFSET, filters.Page * filters.Rows );
            paramDictionary.Add( QUERY_PARAM_QUERY_ROWS, filters.Rows );
        } );
        
        return new DynamicParameters( paramDictionary );
    }

    async Task<IEnumerable<Product>?> ExecuteSearchProducts( SqlConnection connection, string dynamicQuery, DynamicParameters dynamicParams )
    {
        var productDictionary = new Dictionary<int, Product>();
        
        await connection.QueryAsync<Product, ProductVariant, Product>
        ( dynamicQuery, ( product, variant ) =>
            {
                if ( !productDictionary.TryGetValue( product.ProductId, out Product? productEntry ) )
                {
                    productEntry = product;
                    productDictionary.Add( productEntry.ProductId, productEntry );
                }
                if ( variant != null )
                    productEntry.ProductVariants.Add( variant );
                return productEntry;
            },
            dynamicParams,
            splitOn: $"{COLUMN_PRODUCT_ID},{COLUMN_VARIANT_ID}",
            commandType: CommandType.Text );

        return productDictionary.Values;
    }
    async Task<int> CountProducts( SqlConnection connection, string dynamicQuery, DynamicParameters dynamicParams ) { return await connection.QuerySingleAsync<int>( dynamicQuery, dynamicParams, commandType: CommandType.Text ); }
}