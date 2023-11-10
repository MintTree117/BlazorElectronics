using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Repositories.Products;

public sealed class ProductSearchRepository : DapperRepository, IProductSearchRepository
{
    // QUERY PARAM NAMES
    const string PARAM_MIN_RATING = $"@Min{COL_PRODUCT_RATING}";
    const string PARAM_MAX_RATING = $"@Max{COL_PRODUCT_RATING}";
    const string PARAM_MIN_PRICE = "@MinPrice";
    const string PARAM_MAX_PRICE = "@MaxPrice";
    //const string PARAM_QUERY_OFFSET = "@Offset";
    //const string PARAM_QUERY_ROWS = "@Rows";
    const string PARAM_SEARCH_TEXT = "@SearchText";
    const string PARAM_CATEGORY_TIER = $"@{COL_CATEGORY_TIER_ID}";
    const string PARAM_CATEGORY_ID = $"@{COL_CATEGORY_ID}";
    const string PARAM_SPEC_VALUE_ID = "@filterSpec_";
    const string PARAM_SPEC_ID = "@dynamicSpec_";
    const string PARAM_BOOL = "@boolSpec_";

    // STORED PROCEDURES
    const string STORED_PROCEDURE_GET_NAMES_BY_SEARCH_TEXT = "Get_ProductSearchSuggestions";

    enum DynamicSpecCondition
    {
        INCLUDE,
        EXCLUDE
    }
    
    // CONSTRUCTOR
    public ProductSearchRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    // PUBLIC API
    public async Task<IEnumerable<string>?> GetSearchSuggestions( string searchText, int categoryTier, short categoryId )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( PARAM_SEARCH_TEXT, searchText );
        dynamicParams.Add( PARAM_CATEGORY_ID, categoryTier );
        dynamicParams.Add( PARAM_CATEGORY_TIER, categoryId );

        try
        {
            await using SqlConnection connection = await _dbContext.GetOpenConnection();
            return await connection.QueryAsync<string>(
                STORED_PROCEDURE_GET_NAMES_BY_SEARCH_TEXT, dynamicParams, commandType: CommandType.StoredProcedure );
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
    public async Task<string?> GetProductSearchQueryString( CategoryIdMap? categoryIdMap, ProductSearchRequest searchRequest, CachedSpecData specData )
    {
        var productQuery = new StringBuilder();
        var countQuery = new StringBuilder();

        await BuildSearchQueryString( categoryIdMap, searchRequest, specData );
        return productQuery + "-----------------------------------------" + countQuery;
    }
    public async Task<ProductSearch?> GetProductSearch( CategoryIdMap? categoryIdMap, ProductSearchRequest searchRequest, CachedSpecData specData )
    {
        SearchQueryObject searchQueryObject = await BuildSearchQueryString( categoryIdMap, searchRequest, specData );
        string multiSql = $"{searchQueryObject.SearchQuery}; {searchQueryObject.CountQuery}";
        
        return await TryQueryAsync( GetProductSearchQuery, searchQueryObject.DynamicParams, multiSql );
    }
    
    // BUILD & EXECUTE QUERY
    static async Task<ProductSearch?> GetProductSearchQuery( SqlConnection connection, string? sql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( sql, dynamicParams );

        IEnumerable<Product>? products = await multi.ReadAsync<Product>();
        int count = await multi.ReadFirstAsync<int>();

        await multi.DisposeAsync();

        return new ProductSearch {
            Products = products,
            TotalSearchCount = count
        };
    }
    static async Task<SearchQueryObject> BuildSearchQueryString( CategoryIdMap? categoryMap, ProductSearchRequest request, CachedSpecData specData )
    {
        var builder = new StringBuilder();
        var dynamicParams = new DynamicParameters();

        await Task.Run( () =>
        {
            builder.Append( $"SELECT * FROM {TABLE_PRODUCTS}" );

            AppendCategoryJoin( builder, categoryMap );

            if ( request.SpecFilters is not null )
            {
                AppendStaticSpecJoin( builder, TABLE_PRODUCT_SPECS_INT_FILTERS, request.SpecFilters.IntFilters is not null );
                AppendStaticSpecJoin( builder, TABLE_PRODUCT_SPECS_STRING, request.SpecFilters.StringFilters is not null );
                AppendStaticSpecJoin( builder, TABLE_PRODUCT_SPECS_BOOL, request.SpecFilters.BoolFilters is not null );
                AppendDynamicSpecJoins( builder, specData.MultiProductTableNames, request.SpecFilters.MultiIncludes );
                AppendDynamicSpecJoins( builder, specData.MultiProductTableNames, request.SpecFilters.MultiExcludes );   
            }
            
            builder.Append( $" WHERE 1=1" );
            
            AppendCategoryCondition( builder, dynamicParams, categoryMap );
            AppendSearchTextCondition( builder, dynamicParams, request.SearchText );
            AppendHasSaleCondition( builder, request.MustHaveSale );
            AppendRatingConditions( builder, dynamicParams, request.MinRating, request.MaxRating );
            AppendPriceConditions( builder, dynamicParams, request.MinPrice, request.MaxPrice );

            if ( request.SpecFilters is not null )
            {
                AppendStaticSpecConditions( builder, dynamicParams, TABLE_PRODUCT_SPECS_INT_FILTERS, COL_FILTER_INT_ID, request.SpecFilters.IntFilters );
                AppendStaticSpecConditions( builder, dynamicParams, TABLE_PRODUCT_SPECS_STRING, COL_SPEC_VALUE_ID, request.SpecFilters.StringFilters );
                AppendBoolSpecConditions( builder, dynamicParams, request.SpecFilters.BoolFilters );
                AppendDynamicConditions( builder, dynamicParams, DynamicSpecCondition.INCLUDE, specData.MultiProductTableNames, request.SpecFilters.MultiIncludes );
                AppendDynamicConditions( builder, dynamicParams, DynamicSpecCondition.EXCLUDE, specData.MultiProductTableNames, request.SpecFilters.MultiExcludes );
            }
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
    static void AppendStaticSpecJoin( StringBuilder builder, string staticTable, bool filtersExist )
    {
        if ( !filtersExist )
            return;
        
        builder.Append( $" INNER JOIN {staticTable}" );
        builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {staticTable}.{COL_PRODUCT_ID}" );
    }
    static void AppendDynamicSpecJoins( StringBuilder builder, IReadOnlyDictionary<short, string> productSpecTableNames, Dictionary<short, List<short>>? request )
    {
        if ( request is null )
            return;
        
        foreach ( short tableId in productSpecTableNames.Keys )
        {
            if ( !productSpecTableNames.TryGetValue( tableId, out string? tableName ) )
                continue;

            builder.Append( $" INNER JOIN {tableName}" );
            builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {tableName}.{COL_PRODUCT_ID}" );
        }
    }
    
    // APPEND CONDITIONS
    static void AppendCategoryCondition( StringBuilder builder, DynamicParameters dynamicParams, CategoryIdMap? categoryMap )
    {
        if ( categoryMap is null )
            return;
        
        builder.Append( $" AND ( {TABLE_PRODUCT_CATEGORIES}.{COL_CATEGORY_TIER_ID} = {PARAM_CATEGORY_TIER}" );
        builder.Append( $" AND {TABLE_PRODUCT_CATEGORIES}.{COL_CATEGORY_ID} = {PARAM_CATEGORY_ID} )" );

        dynamicParams.Add( PARAM_CATEGORY_TIER, categoryMap.CategoryTier );
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
    
    static void AppendStaticSpecConditions( StringBuilder builder, DynamicParameters dynamicParams, string tableName, string subIdColName, Dictionary<short, List<short>>? request )
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
                clause += $" AND {tableName}.{subIdColName} = {valueIdParamName} )";

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
            string paramName = $"{PARAM_BOOL}{TABLE_PRODUCT_SPECS_BOOL}{specId}";
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_BOOL}.{COL_SPEC_ID} = {paramName}" );
            dynamicParams.Add( paramName, request[ specId ] );
        }
    }
    static void AppendDynamicConditions( StringBuilder builder, DynamicParameters dynamicParams, DynamicSpecCondition conditionType, IReadOnlyDictionary<short, string> productSpecTableNames, Dictionary<short, List<short>>? request )
    {
        if ( request is null )
            return;
        
        string mainCondition = conditionType switch {
            DynamicSpecCondition.INCLUDE => $" AND ",
            DynamicSpecCondition.EXCLUDE => $" AND NOT ",
            _ => throw new ServiceException( $"Invalid DynamicQueryCondition enum encountered!", null )
        };
        
        foreach ( short tableId in request.Keys )
        {
            if ( !productSpecTableNames.TryGetValue( tableId, out string? tableName ) )
                continue;

            List<short> valueIds = request[ tableId ];
            var clauses = new List<string>();
            
            for ( int i = 0; i < valueIds.Count; i++ )
            {
                string paramName = $"{PARAM_SPEC_VALUE_ID}{tableName}{i}";
                clauses.Add( $"{tableName}.{COL_SPEC_VALUE_ID} = {paramName}" );
                dynamicParams.Add( paramName, valueIds[ i ] );
            }

            string joinedClauses = string.Join( " OR ", clauses );
            string finalQuery = $"{mainCondition} ( {joinedClauses} )";
            
            builder.Append( finalQuery );
        }
    }

    sealed class SearchQueryObject
    {
        public string SearchQuery { get; set; } = string.Empty;
        public string CountQuery { get; set; } = string.Empty;
        public DynamicParameters DynamicParams { get; set; } = new();
    }
}