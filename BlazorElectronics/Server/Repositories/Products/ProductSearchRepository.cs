using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Products;

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

            if ( request.SpecFilters is not null )
            {
                AppendSpecJoin( builder, TABLE_PRODUCT_SPECS_INT_FILTERS, request.SpecFilters.IntFilters is not null );
                AppendSpecJoin( builder, TABLE_PRODUCT_SPECS_STRING, request.SpecFilters.StringFilters is not null );
                AppendSpecJoin( builder, TABLE_PRODUCT_SPECS_BOOL, request.SpecFilters.BoolFilters is not null );
                AppendSpecJoin( builder, TABLE_PRODUCT_SPECS_MULTI, request.SpecFilters.MultiIncludes is not null );
            }
            
            builder.Append( $" WHERE 1=1" );
            
            AppendCategoryCondition( builder, dynamicParams, categoryMap );
            AppendSearchTextCondition( builder, dynamicParams, request.SearchText );
            AppendHasSaleCondition( builder, request.MustHaveSale );
            AppendRatingConditions( builder, dynamicParams, request.MinRating, request.MaxRating );
            AppendPriceConditions( builder, dynamicParams, request.MinPrice, request.MaxPrice );

            if ( request.SpecFilters is not null )
            {
                AppendSpecConditions( builder, dynamicParams, TABLE_PRODUCT_SPECS_INT_FILTERS, COL_FILTER_INT_ID, request.SpecFilters.IntFilters );
                AppendSpecConditions( builder, dynamicParams, TABLE_PRODUCT_SPECS_STRING, COL_SPEC_VALUE_ID, request.SpecFilters.StringFilters );
                AppendBoolSpecConditions( builder, dynamicParams, request.SpecFilters.BoolFilters );
                AppendSpecConditions( builder, dynamicParams, TABLE_PRODUCT_SPECS_STRING, COL_SPEC_VALUE_ID, request.SpecFilters.MultiIncludes );
                AppendSpecConditions( builder, dynamicParams, TABLE_PRODUCT_SPECS_STRING, COL_SPEC_VALUE_ID, request.SpecFilters.MultiExcludes );
            }

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
    static void AppendSpecJoin( StringBuilder builder, string tableName, bool filtersExist )
    {
        if ( !filtersExist )
            return;
        
        builder.Append( $" INNER JOIN {tableName}" );
        builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {tableName}.{COL_PRODUCT_ID}" );
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
    static void AppendHasSaleCondition( StringBuilder builder, bool mustHaveSale )
    {
        if ( !mustHaveSale )
            return;
        
        builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_HAS_SALE} = 1" );
    }
    static void AppendRatingConditions( StringBuilder builder, DynamicParameters dynamicParams, int? minRating, int? maxRating )
    {
        if ( minRating.HasValue )
        {
            builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_RATING} >= {PARAM_MIN_RATING}" );
            dynamicParams.Add( PARAM_MIN_RATING, minRating.Value );
        }
        if ( maxRating.HasValue )
        {
            builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_RATING} <= {PARAM_MAX_RATING}" );
            dynamicParams.Add( PARAM_MAX_RATING, maxRating.Value );
        }
    }
    static void AppendPriceConditions( StringBuilder builder, DynamicParameters dynamicParams, int? minPrice, int? maxPrice )
    {
        if ( minPrice.HasValue )
        {
            builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_LOWEST_PRICE} >= {PARAM_MIN_PRICE}" );
            dynamicParams.Add( PARAM_MIN_RATING, minPrice.Value );
        }
        if ( maxPrice.HasValue )
        {
            builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_HIGHEST_PRICE} <= {PARAM_MAX_PRICE}" );
            dynamicParams.Add( PARAM_MAX_RATING, maxPrice.Value );
        }
    }
    
    static void AppendSpecConditions( StringBuilder builder, DynamicParameters dynamicParams, string tableName, string valueIdName, Dictionary<short, List<short>>? request )
    {
        if ( request is null )
            return;
        
        foreach ( short specId in request.Keys )
        {
            List<short> valueIds = request[ specId ];
            var clauses = new List<string>();
            
            for ( int i = 0; i < valueIds.Count; i++ )
            {
                string specIdParamName = $"{PARAM_SPEC_ID}{tableName}{i}";
                string valueIdParamName = $"{PARAM_SPEC_VALUE_ID}{tableName}{i}";

                string clause = $"( {tableName}.{COL_SPEC_ID} = {specIdParamName}";
                clause += $" AND {tableName}.{valueIdName} = {valueIdParamName} )";

                clauses.Add( clause );
                
                dynamicParams.Add( specIdParamName, specId );
                dynamicParams.Add( valueIdParamName, valueIds[ i ] );
            }

            string joinedClauses = string.Join( " OR ", clauses );
            string finalQuery = $" AND ( {joinedClauses} )";
            
            builder.Append( finalQuery );
        }
    }
    static void AppendBoolSpecConditions( StringBuilder builder, DynamicParameters dynamicParams, Dictionary<short, bool>? request )
    {
        if ( request is null )
            return;

        foreach ( short specId in request.Keys )
        {
            string idParamName = $"{PARAM_BOOL}{TABLE_PRODUCT_SPECS_BOOL}{specId}";
            string valueParamName = $"{PARAM_SPEC_VALUE}{TABLE_PRODUCT_SPECS_BOOL}{specId}";
            
            builder.Append( $" AND ( {TABLE_PRODUCT_SPECS_BOOL}.{COL_SPEC_ID} = {idParamName}" );
            builder.Append( $" {TABLE_PRODUCT_SPECS_BOOL}.{COL_SPEC_VALUE} = {valueParamName} )" );
                
            dynamicParams.Add( idParamName, specId);
            dynamicParams.Add( valueParamName, request[ specId ] );
        }
    }

    sealed class SearchQueryObject
    {
        public string SearchQuery { get; init; } = string.Empty;
        public DynamicParameters DynamicParams { get; init; } = new();
    }
}