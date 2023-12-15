using System.Text;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Products;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Products.Search;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Data.Repositories;

public sealed class ProductSearchRepository : DapperRepository, IProductSearchRepository
{
    // QUERY PARAM NAMES
    const string PARAM_MIN_RATING = $"@Min{COL_PRODUCT_RATING}";
    const string PARAM_MIN_PRICE = "@MinPrice";
    const string PARAM_MAX_PRICE = "@MaxPrice";
    const string PARAM_QUERY_OFFSET = "@Offset";
    const string PARAM_QUERY_ROWS = "@Rows";
    const string PARAM_SEARCH_TEXT = "@SearchText";
    const string PARAM_SPEC_VALUE_ID = "@filterSpec_";

    // STORED PROCEDURES
    const string STORED_PROCEDURE_GET_SUGGESTIONS = "Get_ProductSuggestions";

    // CONSTRUCTOR
    public ProductSearchRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    // PUBLIC API
    public async Task<IEnumerable<string>?> GetSearchSuggestions( string searchText )
    {
        DynamicParameters p = new();
        p.Add( PARAM_SEARCH_TEXT, searchText );
        return await TryQueryAsync( Query<string>, p, STORED_PROCEDURE_GET_SUGGESTIONS );
    }
    public async Task<string?> GetProductSearchQuery( ProductSearchRequest searchRequest )
    {
        SearchQueryObject query = await BuildProductSearchQuery( searchRequest );
        return query.SearchQuery;
    }
    public async Task<IEnumerable<ProductSearchModel>?> GetProductSearch( ProductSearchRequest searchRequest )
    {
        SearchQueryObject searchQueryObject = await BuildProductSearchQuery( searchRequest );
        return await TryQueryAsync( GetProductSearchQuery, searchQueryObject.DynamicParams, searchQueryObject.SearchQuery );
    }
    
    // BUILD & EXECUTE QUERY
    static async Task<IEnumerable<ProductSearchModel>?> GetProductSearchQuery( SqlConnection connection, string? sql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<ProductSearchModel>( sql, dynamicParams );
    }
    static async Task<SearchQueryObject> BuildProductSearchQuery( ProductSearchRequest request )
    {
        var builder = new StringBuilder();
        var dynamicParams = new DynamicParameters();
        ProductSearchFilters? filters = request.Filters;

        await Task.Run( () =>
        {
            const string pSelection = $"{TABLE_PRODUCTS}.{COL_PRODUCT_ID}," +
                                      $"{TABLE_PRODUCTS}.{COL_VENDOR_ID}," +
                                      $"{TABLE_PRODUCTS}.{COL_PRODUCT_TITLE}," +
                                      $"{TABLE_PRODUCTS}.{COL_PRODUCT_THUMBNAIL}," +
                                      $"{TABLE_PRODUCTS}.{COL_PRODUCT_RELEASE_DATE}," +
                                      $"{TABLE_PRODUCTS}.{COL_PRODUCT_PRICE}," +
                                      $"{TABLE_PRODUCTS}.{COL_PRODUCT_SALE_PRICE}," +
                                      $"{TABLE_PRODUCTS}.{COL_PRODUCT_IS_FEATURED}," +
                                      $"{TABLE_PRODUCTS}.{COL_PRODUCT_NUMBER_SOLD}," +
                                      $"{TABLE_PRODUCTS}.{COL_PRODUCT_NUMBER_REVIEWS}";
            
            builder.Append( $"WITH Results AS (" );
            builder.Append( $" SELECT DISTINCT {TABLE_PRODUCTS}.*, TotalCount = COUNT(*) OVER() FROM {TABLE_PRODUCTS}" );

            AppendCategoryJoin( builder, request.CategoryId );
            AppendSpecLookupJoin( builder, filters?.SpecsInclude is not null || filters?.SpecsExlude is not null );
            
            builder.Append( $" WHERE 1=1" );
            
            AppendCategoryCondition( builder, dynamicParams, request.CategoryId );
            AppendSearchTextCondition( builder, dynamicParams, request.SearchText );

            if ( filters is not null )
            {
                AppendVendorCondition( builder, dynamicParams, filters.Vendors );
                AppendHasSaleCondition( builder, filters.OnSale );
                AppendRatingConditions( builder, dynamicParams, filters.MinRating );
                AppendPriceConditions( builder, dynamicParams, filters.MinPrice, filters.MaxPrice );
                AppendSpecConditions( builder, dynamicParams, filters.SpecsInclude, false );
                AppendSpecConditions( builder, dynamicParams, filters.SpecsExlude, true );
            }

            builder.Append( " )" );
            builder.Append( $" SELECT *, TotalCount" );
            builder.Append( $" FROM Results" );
            AppendSort( builder, request.SortType );
            builder.Append( $" OFFSET {PARAM_QUERY_OFFSET} ROWS" );
            builder.Append( $" FETCH NEXT {PARAM_QUERY_ROWS} ROWS ONLY;" );

            dynamicParams.Add( PARAM_QUERY_OFFSET, Math.Max( 0, request.Page - 1 ) * request.Rows );
            dynamicParams.Add( PARAM_QUERY_ROWS, request.Rows );
        } );
        
        return new SearchQueryObject() 
        {
            SearchQuery = builder.ToString(),
            DynamicParams = dynamicParams
        };
    }
    
    // APPEND JOINS
    static void AppendCategoryJoin( StringBuilder builder, int? categoryId )
    {
        if ( categoryId is null )
            return;
        
        builder.Append( $" INNER JOIN {TABLE_PRODUCT_CATEGORIES}" );
        builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {TABLE_PRODUCT_CATEGORIES}.{COL_PRODUCT_ID}" );   
    }
    static void AppendSpecLookupJoin( StringBuilder builder, bool filtersExist )
    {
        if ( !filtersExist )
            return;
        
        builder.Append( $" INNER JOIN {TABLE_PRODUCT_SPECS}" );
        builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {TABLE_PRODUCT_SPECS}.{COL_PRODUCT_ID}" );
    }

    // APPEND CONDITIONS
    static void AppendCategoryCondition( StringBuilder builder, DynamicParameters dynamicParams, int? categoryId )
    {
        if ( categoryId is null )
            return;
        
        builder.Append( $" AND {TABLE_PRODUCT_CATEGORIES}.{COL_CATEGORY_ID} = {PARAM_CATEGORY_ID}" );
        dynamicParams.Add( PARAM_CATEGORY_ID, categoryId.Value );
    }
    static void AppendSearchTextCondition( StringBuilder builder, DynamicParameters dynamicParams, string? searchText )
    {
        if ( string.IsNullOrWhiteSpace( searchText ) )
            return;
        
        builder.Append( $" AND ( {TABLE_PRODUCTS}.{COL_PRODUCT_TITLE} LIKE {PARAM_SEARCH_TEXT}" );
        builder.Append( $" OR {TABLE_PRODUCT_DESCRIPTIONS}.{COL_PRODUCT_DESCR} LIKE {PARAM_SEARCH_TEXT} )" );
        dynamicParams.Add( PARAM_SEARCH_TEXT, searchText );
    }
    static void AppendVendorCondition( StringBuilder builder, DynamicParameters dynamicParams, List<int>? vendors )
    {
        if ( vendors is null || vendors.Count <= 0 )
            return;

        var inParams = new List<string>();
        for ( int i = 0; i < vendors.Count; i++ )
        {
            string paramName = $"{PARAM_VENDOR_ID}{i}";
            inParams.Add( paramName );
            dynamicParams.Add( paramName, vendors[ i ] );
        }
        
        string valuesClause = string.Join( ", ", inParams );
        string finalQuery = $" AND {TABLE_PRODUCTS}.{COL_VENDOR_ID} IN ({valuesClause})";

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
    static void AppendSpecConditions( StringBuilder builder, DynamicParameters dynamicParams, Dictionary<int, List<int>>? specs, bool exclude )
    {
        if ( specs is null || specs.Count <=0 )
            return;

        string condition = exclude
            ? " NOT IN "
            : " IN ";

        foreach ( (int specId, List<int>? valueIds) in specs )
        {
            // Create parameter names for IN clause
            var inParams = new List<string>();
            for ( int i = 0; i < valueIds.Count; i++ )
            {
                string paramName = $"{PARAM_SPEC_VALUE_ID}{TABLE_PRODUCT_SPECS}{i}";
                inParams.Add( paramName );
                dynamicParams.Add( paramName, valueIds[ i ] );
            }

            string valuesClause = string.Join( ", ", inParams );

            // Append the condition with IN clause
            string finalQuery = $" AND ({TABLE_PRODUCT_SPECS}.{COL_SPEC_ID} = {PARAM_SPEC_ID}{TABLE_PRODUCT_SPECS})";
            finalQuery += $" AND {TABLE_PRODUCT_SPECS}.{COL_SPEC_VALUE_ID} {condition} ({valuesClause})";

            builder.Append( finalQuery );
            dynamicParams.Add( $"{PARAM_SPEC_ID}{TABLE_PRODUCT_SPECS}", specId );
        }
    }
    static void AppendSort( StringBuilder builder, ProductSortType sortType )
    {
        switch ( sortType )
        {
            case ProductSortType.Featured:
                builder.Append( $" ORDER BY {COL_PRODUCT_IS_FEATURED}" );
                break;
            case ProductSortType.LowestPrice:
                builder.Append( $" ORDER BY {COL_PRODUCT_PRICE}" );
                break;
            case ProductSortType.HighestPrice:
                builder.Append( $" ORDER BY {COL_PRODUCT_PRICE} DESC" );
                break;
            case ProductSortType.BestRating:
                builder.Append( $" ORDER BY {COL_PRODUCT_RATING} DESC" );
                break;
            case ProductSortType.BestSelling:
                builder.Append( $" ORDER BY {COL_PRODUCT_NUMBER_SOLD} DESC" );
                break;
            case ProductSortType.MostReviews:
                builder.Append( $" ORDER BY {COL_PRODUCT_NUMBER_REVIEWS} DESC" );
                break;
            default: throw new ArgumentOutOfRangeException( nameof( sortType ), sortType, null );
        }
    }

    sealed class SearchQueryObject
    {
        public string SearchQuery { get; init; } = string.Empty;
        public DynamicParameters DynamicParams { get; init; } = new();
    }
}