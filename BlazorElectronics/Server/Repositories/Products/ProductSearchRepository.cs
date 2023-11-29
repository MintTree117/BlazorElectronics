using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Repositories.Products;

public sealed class ProductSearchRepository : DapperRepository, IProductSearchRepository
{
    // QUERY PARAM NAMES
    const string PARAM_MIN_RATING = $"@Min{COL_PRODUCT_RATING}";
    const string PARAM_MAX_RATING = $"@Max{COL_PRODUCT_RATING}";
    const string PARAM_MIN_PRICE = "@MinPrice";
    const string PARAM_MAX_PRICE = "@MaxPrice";
    const string PARAM_QUERY_OFFSET = "@Offset";
    const string PARAM_QUERY_ROWS = "@Rows";
    const string PARAM_SEARCH_TEXT = "@SearchText";
    const string PARAM_SPEC_VALUE_ID = "@filterSpec_";
    const string PARAM_SPEC_ID = "@dynamicSpec_";
    const string PARAM_SPEC_VALUE = "@specValue_";
    const string PARAM_BOOL = "@boolSpec_";

    // STORED PROCEDURES
    const string STORED_PROCEDURE_GET_SEARCH_SUGGESTIONS = "Get_ProductSearchSuggestions";

    // CONSTRUCTOR
    public ProductSearchRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    // PUBLIC API
    public async Task<IEnumerable<string>?> GetSearchSuggestions( string searchText, CategoryType categoryType, short categoryId )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( PARAM_SEARCH_TEXT, searchText );
        dynamicParams.Add( PARAM_CATEGORY_ID, categoryType );
        dynamicParams.Add( PARAM_CATEGORY_TYPE, categoryId );

        try
        {
            await using SqlConnection connection = await _dbContext.GetOpenConnection();
            return await connection.QueryAsync<string>(
                STORED_PROCEDURE_GET_SEARCH_SUGGESTIONS, dynamicParams, commandType: CommandType.StoredProcedure );
        }
        catch ( SqlException e )
        {
            throw new ServiceException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new ServiceException( e.Message, e );
        }
    }
    public async Task<string?> GetProductSearchQueryString( CategoryIdMap? categoryIdMap, ProductSearchRequest searchRequest )
    {
        var productQuery = new StringBuilder();
        var countQuery = new StringBuilder();

        await BuildProductSearchQuery( categoryIdMap, searchRequest );
        return productQuery + "-----------------------------------------" + countQuery;
    }
    public async Task<IEnumerable<ProductSearchModel>?> GetProductSearch( CategoryIdMap? categoryIdMap, ProductSearchRequest searchRequest )
    {
        SearchQueryObject searchQueryObject = await BuildProductSearchQuery( categoryIdMap, searchRequest );
        return await TryQueryAsync( GetProductSearchQuery, searchQueryObject.DynamicParams, searchQueryObject.SearchQuery );
    }
    
    // BUILD & EXECUTE QUERY
    static async Task<IEnumerable<ProductSearchModel>?> GetProductSearchQuery( SqlConnection connection, string? sql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<ProductSearchModel>( sql, dynamicParams );
    }
    static async Task<SearchQueryObject> BuildProductSearchQuery( CategoryIdMap? categoryMap, ProductSearchRequest request )
    {
        var builder = new StringBuilder();
        var dynamicParams = new DynamicParameters();

        await Task.Run( () =>
        {
            builder.Append( $"WITH Results AS (" );
            builder.Append( $"SELECT *, TotalCount = COUNT(*) OVER() FROM {TABLE_PRODUCTS}" );

            AppendCategoryJoin( builder, categoryMap );
            AppendSpecLookupJoin( builder, request.LookupIncludes is not null || request.LookupExcludes is not null );
            
            builder.Append( $" WHERE 1=1" );
            
            AppendCategoryCondition( builder, dynamicParams, categoryMap );
            AppendSearchTextCondition( builder, dynamicParams, request.SearchText );
            AppendHasSaleCondition( builder, request.HasSale );
            AppendRatingConditions( builder, dynamicParams, request.MinRating );
            AppendPriceConditions( builder, dynamicParams, request.MinPrice, request.MaxPrice );
            AppendLookupConditions( builder, dynamicParams, request.LookupIncludes, false );
            AppendLookupConditions( builder, dynamicParams, request.LookupExcludes, true );

            builder.Append( " )" );
            builder.Append( $" SELECT *, TotalCount" );
            builder.Append( $" FROM Results" );
            builder.Append( $" ORDER BY {COL_PRODUCT_ID}" );
            builder.Append( $" OFFSET {PARAM_QUERY_OFFSET} ROWS" );
            builder.Append( $" FETCH NEXT {PARAM_QUERY_ROWS} ROWS ONLY;" );

            dynamicParams.Add( PARAM_QUERY_OFFSET, Math.Max( 0, request.Page - 1 ) );
            dynamicParams.Add( PARAM_QUERY_ROWS, request.Rows );
        } );
        
        return new SearchQueryObject() 
        {
            SearchQuery = builder.ToString(),
            DynamicParams = dynamicParams
        };
    }
    
    // APPEND JOINS
    static void AppendCategoryJoin( StringBuilder builder, CategoryIdMap? categoryMap )
    {
        if ( categoryMap is null )
            return;
        
        builder.Append( $" INNER JOIN {TABLE_PRODUCT_CATEGORIES}" );
        builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {TABLE_PRODUCT_CATEGORIES}.{COL_PRODUCT_ID}" );   
    }
    static void AppendSpecLookupJoin( StringBuilder builder, bool filtersExist )
    {
        if ( !filtersExist )
            return;
        
        builder.Append( $" INNER JOIN {TABLE_PRODUCT_SPEC_LOOKUPS}" );
        builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {TABLE_PRODUCT_SPEC_LOOKUPS}.{COL_PRODUCT_ID}" );
    }

    // APPEND CONDITIONS
    static void AppendCategoryCondition( StringBuilder builder, DynamicParameters dynamicParams, CategoryIdMap? categoryMap )
    {
        if ( categoryMap is null )
            return;
        
        builder.Append( $" AND ( {TABLE_PRODUCT_CATEGORIES}.{COL_CATEGORY_TIER_ID} = {PARAM_CATEGORY_TYPE}" );
        builder.Append( $" AND {TABLE_PRODUCT_CATEGORIES}.{COL_CATEGORY_ID} = {PARAM_CATEGORY_ID} )" );

        dynamicParams.Add( PARAM_CATEGORY_TYPE, categoryMap.CategoryType );
        dynamicParams.Add( PARAM_CATEGORY_ID, categoryMap.CategoryId );
    }
    static void AppendSearchTextCondition( StringBuilder builder, DynamicParameters dynamicParams, string? searchText )
    {
        if ( string.IsNullOrWhiteSpace( searchText ) )
            return;
        
        builder.Append( $" AND ( {TABLE_PRODUCTS}.{COL_PRODUCT_TITLE} LIKE {PARAM_SEARCH_TEXT}" );
        builder.Append( $" OR {TABLE_PRODUCT_DESCRIPTIONS}.{COL_PRODUCT_DESCR_BODY} LIKE {PARAM_SEARCH_TEXT} )" );
        dynamicParams.Add( PARAM_SEARCH_TEXT, searchText );
    }
    static void AppendVendorCondition( StringBuilder builder, DynamicParameters dynamicParams, List<int>? vendors )
    {
        if ( vendors is null )
            return;

        var inParams = new List<string>();
        for ( int i = 0; i < vendors.Count; i++ )
        {
            string paramName = $"{PARAM_VENDOR_ID}{i}";
            inParams.Add( paramName );
            dynamicParams.Add( paramName, vendors[ i ] );
        }
        
        string valuesClause = string.Join( ", ", inParams.Select( p => "@" + p ) );
        string finalQuery = $"AND {TABLE_PRODUCTS}.{COL_VENDOR_ID} IN ({valuesClause})";

        builder.Append( finalQuery );
    }
    static void AppendHasSaleCondition( StringBuilder builder, bool? mustHaveSale )
    {
        if ( !mustHaveSale.HasValue || !mustHaveSale.Value )
            return;
        
        builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_SALE_PRICE} IS NOT NULL" );
    }
    static void AppendRatingConditions( StringBuilder builder, DynamicParameters dynamicParams, int? minRating )
    {
        if ( !minRating.HasValue ) 
            return;
        
        builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_RATING} >= {PARAM_MIN_RATING}" );
        dynamicParams.Add( PARAM_MIN_RATING, minRating.Value );
    }
    static void AppendPriceConditions( StringBuilder builder, DynamicParameters dynamicParams, int? minPrice, int? maxPrice )
    {
        const string priceColumn = $"COALESCE({TABLE_PRODUCTS}.{COL_PRODUCT_SALE_PRICE}, {TABLE_PRODUCTS}.{COL_PRODUCT_PRICE})";

        if ( minPrice.HasValue )
        {
            builder.Append( $" AND {priceColumn} >= {PARAM_MIN_PRICE}" );
            dynamicParams.Add( PARAM_MIN_PRICE, minPrice.Value );
        }
        if ( maxPrice.HasValue )
        {
            builder.Append( $" AND {priceColumn} <= {PARAM_MAX_PRICE}" );
            dynamicParams.Add( PARAM_MAX_PRICE, maxPrice.Value );
        }
    }
    static void AppendLookupConditions( StringBuilder builder, DynamicParameters dynamicParams, Dictionary<int, List<int>>? lookups, bool exclude )
    {
        if ( lookups is null )
            return;

        string condition = exclude
            ? " NOT IN "
            : " IN ";

        foreach ( (int specId, List<int>? valueIds) in lookups )
        {
            // Create parameter names for IN clause
            var inParams = new List<string>();
            for ( int i = 0; i < valueIds.Count; i++ )
            {
                string paramName = $"{PARAM_SPEC_VALUE_ID}{TABLE_PRODUCT_SPEC_LOOKUPS}{i}";
                inParams.Add( paramName );
                dynamicParams.Add( paramName, valueIds[ i ] );
            }

            string valuesClause = string.Join( ", ", inParams.Select( p => "@" + p ) );

            // Append the condition with IN clause
            string finalQuery = $" AND ({TABLE_PRODUCT_SPEC_LOOKUPS}.{COL_SPEC_ID} = @{PARAM_SPEC_ID}{TABLE_PRODUCT_SPEC_LOOKUPS})";
            finalQuery += $" AND {TABLE_PRODUCT_SPEC_LOOKUPS}.{COL_SPEC_VALUE_ID} {condition} ({valuesClause})";

            builder.Append( finalQuery );
            dynamicParams.Add( $"{PARAM_SPEC_ID}{TABLE_PRODUCT_SPEC_LOOKUPS}", specId );
        }
    }

    sealed class SearchQueryObject
    {
        public string SearchQuery { get; init; } = string.Empty;
        public DynamicParameters DynamicParams { get; init; } = new();
    }
}