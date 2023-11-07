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
    const string PARAM_PRODUCT_ID = $"@{COL_PRODUCT_ID}";
    const string PARAM_MIN_RATING = $"@Min{COL_PRODUCT_RATING}";
    const string PARAM_MAX_RATING = $"@Max{COL_PRODUCT_RATING}";
    const string PARAM_MIN_PRICE = "@MinPrice";
    const string PARAM_MAX_PRICE = "@MaxPrice";
    const string PARAM_QUERY_OFFSET = "@Offset";
    const string PARAM_QUERY_ROWS = "@Rows";
    const string PARAM_SEARCH_TEXT = "@SearchText";
    const string PARAM_CATEGORY_TIER = $"@{COL_CATEGORY_TIER_ID}";
    const string PARAM_CATEGORY_ID = $"@{COL_CATEGORY_ID}";
    const string PARAM_EXPLICIT_SPEC = "@explicitSpec_";
    const string PARAM_DYNAMIC_SPEC = "@dynamicSpec_";

    // STORED PROCEDURES
    const string STORED_PROCEDURE_GET_NAMES_BY_SEARCH_TEXT = "Get_ProductSearchSuggestions";

    enum ExplicitQueryCondition
    {
        LESS_THAN,
        GREATER_THAN,
        EQUAL_TO
    }

    enum DynamicQueryCondition
    {
        INCLUDE,
        EXCLUDE
    }

    public ProductSearchRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<string?> GetProductSearchQueryString( ProductSearchRequest searchRequest )
    {
        var productQuery = new StringBuilder();
        var countQuery = new StringBuilder();

        //await BuildProductSearchQuery( searchRequest, productQuery, countQuery );
        return productQuery + "-----------------------------------------" + countQuery;
    }
    
    public async Task<IEnumerable<string>?> GetSearchSuggestions( string searchText, int categoryTier, short categoryId )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( PARAM_SEARCH_TEXT, searchText );
        dynamicParams.Add( PARAM_CATEGORY_ID, categoryTier );
        dynamicParams.Add( PARAM_CATEGORY_TIER, categoryId );

        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
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
    public async Task<ProductSearch?> GetProductSearch( CategoryIdMap categoryIdMap, ProductSearchRequest searchRequest, SpecLookupTableMetaDto specTableMeta )
    {
        SearchQueryObject searchQueryObject = await BuildSearchQueryString( categoryIdMap, searchRequest, specTableMeta );
        string multiSql = $"{searchQueryObject.SearchQuery}; {searchQueryObject.CountQuery}";
        
        return await TryQueryAsync( GetProductSearchQuery, searchQueryObject.DynamicParams, multiSql );
    }
    
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
    static async Task<SearchQueryObject> BuildSearchQueryString( CategoryIdMap? categoryMap, ProductSearchRequest request, SpecLookupTableMetaDto specTableMeta )
    {
        var builder = new StringBuilder();
        var dynamicParams = new DynamicParameters();

        await Task.Run( () =>
        {
            builder.Append( $"SELECT * FROM {TABLE_PRODUCTS}" );

            AppendCategoryJoin( builder, categoryMap );
            AppendSpecFilterJoins( builder, request.SpecFilters, specTableMeta );
            AppendCategoryCondition( builder, dynamicParams, categoryMap );
            AppendSearchTextCondition( builder, dynamicParams, request.SearchText );
            AppendHasSaleCondition( builder, request.MustHaveSale );
            AppendRatingConditions( builder, dynamicParams, request.MinRating, request.MaxRating );
            AppendPriceConditions( builder, dynamicParams, request.MinPrice, request.MaxPrice );
            AppendSpecFilterConditions( builder, dynamicParams, request.SpecFilters, specTableMeta );

        } );
        
        return new SearchQueryObject() 
        {
            SearchQuery = builder.ToString(),
            DynamicParams = dynamicParams
        };
    }
    
    static void AppendCategoryJoin( StringBuilder builder, CategoryIdMap? categoryMap )
    {
        if ( categoryMap == null )
            return;
        
        builder.Append( $" INNER JOIN {TABLE_PRODUCT_CATEGORIES}" );
        builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {TABLE_PRODUCT_CATEGORIES}.{COL_PRODUCT_ID}" );   
    }
    static void AppendSpecFilterJoins( StringBuilder builder, ProductSearchRequestSpecFilters? specFilters, SpecLookupTableMetaDto specTableMeta )
    {
        if ( specFilters == null )
            return;

        if ( specFilters.ExplicitIntSpecsMinimums is not null || specFilters.ExplicitIntSpecsMaximums is not null )
        {
            AppendExplicitQueryJoin( builder, TABLE_PRODUCT_SPECS_EXPLICIT_INT );
        }
        if ( specFilters.ExplicitStringSpecsEqualities is not null )
        {
            AppendExplicitQueryJoin( builder, TABLE_PRODUCT_SPECS_EXPLICIT_STRING );
        }

        // DYNAMIC SPEC JOINS
        if ( specFilters.DynamicSpecsIncludeIdsByTable is not null )
        {
            AppendDynamicQueryJoins( builder, specFilters.DynamicSpecsIncludeIdsByTable, specTableMeta.DynamicProductTableNames );
        }
        if ( specFilters.DynamicSpecsExcludeIdsByTable is not null )
        {
            AppendDynamicQueryJoins( builder, specFilters.DynamicSpecsExcludeIdsByTable, specTableMeta.DynamicProductTableNames );
        }
    }
    static void AppendExplicitQueryJoin( StringBuilder builder, string explicitTable )
    {
        builder.Append( $" INNER JOIN {explicitTable}" );
        builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {explicitTable}.{COL_PRODUCT_ID}" );
    }
    static void AppendDynamicQueryJoins( StringBuilder builder, Dictionary<short, List<short>> requestSpecIdsByTableId, Dictionary<short, string> productSpecTableNamesBySpecTableId )
    {
        foreach ( short tableId in requestSpecIdsByTableId.Keys )
        {
            if ( !productSpecTableNamesBySpecTableId.TryGetValue( tableId, out string? productSpecTable ) )
                continue;

            builder.Append( $" INNER JOIN {productSpecTable}" );
            builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {productSpecTable}.{COL_PRODUCT_ID}" );
        }
    }
    
    static void AppendCategoryCondition( StringBuilder builder, DynamicParameters dynamicParams, CategoryIdMap? categoryMap )
    {
        if ( categoryMap == null )
            return;
        
        builder.Append( $" WHERE {TABLE_PRODUCT_CATEGORIES}.{COL_CATEGORY_TIER_ID} = {PARAM_CATEGORY_TIER}" );
        builder.Append( $" AND {TABLE_PRODUCT_CATEGORIES}.{COL_CATEGORY_ID} = {PARAM_CATEGORY_ID}" );

        dynamicParams.Add( PARAM_CATEGORY_TIER, categoryMap.Tier );
        dynamicParams.Add( PARAM_CATEGORY_ID, categoryMap.CategoryId );
    }
    static void AppendSearchTextCondition( StringBuilder builder, DynamicParameters dynamicParams, string? searchText )
    {
        if ( string.IsNullOrWhiteSpace( searchText ) )
            return;
        
        builder.Append( $" AND ( {TABLE_PRODUCTS}.{COL_PRODUCT_TITLE} LIKE {searchText}" );
        builder.Append( $" OR ( {TABLE_PRODUCT_DESCRIPTIONS}.{COL_PRODUCT_DESCR_BODY} LIKE {searchText} )" );
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
            builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_RATING} > {PARAM_MIN_RATING}" );
            dynamicParams.Add( PARAM_MIN_RATING, minRating.Value );
        }
        if ( maxRating.HasValue )
        {
            builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_RATING} < {PARAM_MAX_RATING}" );
            dynamicParams.Add( PARAM_MAX_RATING, maxRating.Value );
        }
    }
    static void AppendPriceConditions( StringBuilder builder, DynamicParameters dynamicParams, int? minPrice, int? maxPrice )
    {
        if ( minPrice.HasValue )
        {
            builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_LOWEST_PRICE} > {PARAM_MIN_PRICE}" );
            dynamicParams.Add( PARAM_MIN_RATING, minPrice.Value );
        }
        if ( maxPrice.HasValue )
        {
            builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_HIGHEST_PRICE} < {PARAM_MAX_PRICE}" );
            dynamicParams.Add( PARAM_MAX_RATING, maxPrice.Value );
        }
    }
    static void AppendSpecFilterConditions( StringBuilder builder, DynamicParameters dynamicParams, ProductSearchRequestSpecFilters? specFilters, SpecLookupTableMetaDto specTableMeta )
    {
        if ( specFilters == null )
            return;

        // EXPLICIT CONDITIONS
        if ( specFilters.ExplicitIntSpecsMinimums != null )
        {
            AppendExplicitQueryConditions( builder, dynamicParams, ExplicitQueryCondition.GREATER_THAN, TABLE_PRODUCT_SPECS_EXPLICIT_INT, specFilters.ExplicitIntSpecsMinimums, specTableMeta.ExplicitIntNames );
        }
        if ( specFilters.ExplicitIntSpecsMaximums != null )
        {
            AppendExplicitQueryConditions( builder, dynamicParams, ExplicitQueryCondition.LESS_THAN, TABLE_PRODUCT_SPECS_EXPLICIT_INT, specFilters.ExplicitIntSpecsMaximums, specTableMeta.ExplicitIntNames );
        }
        if ( specFilters.ExplicitStringSpecsEqualities != null )
        {
            AppendExplicitQueryConditions( builder, dynamicParams, ExplicitQueryCondition.EQUAL_TO, TABLE_PRODUCT_SPECS_EXPLICIT_STRING, specFilters.ExplicitStringSpecsEqualities, specTableMeta.ExplicitStringNames );
        }

        // DYNAMIC CONDITIONS
        if ( specFilters.DynamicSpecsIncludeIdsByTable != null )
        {
            AppendDynamicQueryConditions( builder, dynamicParams, DynamicQueryCondition.INCLUDE, specFilters.DynamicSpecsIncludeIdsByTable, specTableMeta.DynamicProductTableNames );
        }
        if ( specFilters.DynamicSpecsExcludeIdsByTable != null )
        {
            AppendDynamicQueryConditions( builder, dynamicParams, DynamicQueryCondition.EXCLUDE, specFilters.DynamicSpecsExcludeIdsByTable, specTableMeta.DynamicProductTableNames );
        }
    }
    static void AppendExplicitQueryConditions<T>( StringBuilder builder, DynamicParameters dynamicParams, ExplicitQueryCondition conditionType, string explicitTable, Dictionary<short, T> explicitRequests, Dictionary<short, string> explicitSpecNamesBySpecId )
    {
        string? condition = conditionType switch {
            ExplicitQueryCondition.LESS_THAN => "<=",
            ExplicitQueryCondition.GREATER_THAN => ">=",
            ExplicitQueryCondition.EQUAL_TO => "=",
            _ => throw new ServiceException( $"Invalid ExplicitQueryCondition enum encountered!", null )
        };

        foreach ( short explicitId in explicitRequests.Keys )
        {
            if ( !explicitSpecNamesBySpecId.TryGetValue( explicitId, out string? specName ) )
                continue;

            string paramName = $"{PARAM_EXPLICIT_SPEC}{specName}";

            builder.Append( $" AND {explicitTable}.{COL_PRODUCT_SPEC_EXPLICIT_VALUE} {condition} {paramName}" );
            dynamicParams.Add( paramName, explicitRequests[ explicitId ] );
        }
    }
    static void AppendDynamicQueryConditions( StringBuilder builder, DynamicParameters dynamicParams, DynamicQueryCondition conditionType, Dictionary<short, List<short>> requestSpecIdsByTableId, Dictionary<short, string> productSpecTableNamesBySpecTableId )
    {
        string mainCondition = conditionType switch {
            DynamicQueryCondition.INCLUDE => $" AND ",
            DynamicQueryCondition.EXCLUDE => $" AND NOT ",
            _ => throw new ServiceException( $"Invalid DynamicQueryCondition enum encountered!", null )
        };
        
        foreach ( short tableId in requestSpecIdsByTableId.Keys )
        {
            if ( !productSpecTableNamesBySpecTableId.TryGetValue( tableId, out string? tableName ) )
                continue;

            List<short> requestSpecValues = requestSpecIdsByTableId[ tableId ];
            var clauses = new List<string>();
            
            for ( int i = 0; i < requestSpecValues.Count; i++ )
            {
                string paramName = $"{PARAM_DYNAMIC_SPEC}{tableName}{i}";
                clauses.Add( $"{tableName}.{COL_PRODUCT_SPEC_DYNAMIC_ID} = {paramName}" );
                dynamicParams.Add( paramName, requestSpecValues[ i ] );
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