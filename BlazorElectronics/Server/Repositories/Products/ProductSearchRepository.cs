using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Repositories.Products;

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

    const string PARAM_HAS_SUBTITLES = "@HasSubtitles";
    const string PARAM_MIN_FILESIZE = "@FileSizeMin";
    const string PARAM_MAX_FILESIZE = "@FileSizeMax";
    
    const string PARAM_BOOK_MIN_PAGES = "@BookPagesMin";
    const string PARAM_BOOK_MAX_PAGES = "@BookPagesMax";
    const string PARAM_BOOK_HAS_AUDIO = "@BookHasAudio";
    const string PARAM_BOOK_MIN_AUDIO = "@BookAudioMin";
    const string PARAM_BOOK_MAX_AUDIO = "@BookAudioMax";

    const string PARAM_GAME_HAS_MULTIPLAYER = "@HasMultiplayer";
    const string PARAM_GAME_HAS_OFFLINE = "@HasOffline";
    const string PARAM_GAME_HAS_CONTROLLER = "@HasController";
    const string PARAM_GAME_HAS_PURCHASES = "@HasPurchases";

    const string PARAM_VIDEO_MIN_RUNTIME = "@RuntimeMin";
    const string PARAM_VIDEO_MAX_RUNTIME = "@RuntimeMax";
    const string PARAM_VIDEO_MIN_EPISODES = "@EpisodesMin";
    const string PARAM_VIDEO_MAX_EPISODES = "@EpisodesMax";

    const string PARAM_COURSE_MIN_DURATION = "@MinDuration";
    const string PARAM_COURSE_MAX_DURATION = "@MaxDuration";

    // STORED PROCEDURES
    const string STORED_PROCEDURE_GET_SEARCH_SUGGESTIONS = "Get_ProductSearchSuggestions";

    // CONSTRUCTOR
    public ProductSearchRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    // PUBLIC API
    public async Task<IEnumerable<string>?> GetSearchSuggestions( string searchText )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( PARAM_SEARCH_TEXT, searchText );

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
    public async Task<IEnumerable<ProductSearchModel>?> GetProductSearch( int? categoryId, ProductSearchRequest searchRequest )
    {
        SearchQueryObject searchQueryObject = await BuildProductSearchQuery( categoryId, searchRequest );
        return await TryQueryAsync( GetProductSearchQuery, searchQueryObject.DynamicParams, searchQueryObject.SearchQuery );
    }
    
    // BUILD & EXECUTE QUERY
    static async Task<IEnumerable<ProductSearchModel>?> GetProductSearchQuery( SqlConnection connection, string? sql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<ProductSearchModel>( sql, dynamicParams );
    }
    static async Task<SearchQueryObject> BuildProductSearchQuery( int? categoryId, ProductSearchRequest request )
    {
        var builder = new StringBuilder();
        var dynamicParams = new DynamicParameters();

        await Task.Run( () =>
        {
            builder.Append( $"WITH Results AS (" );
            builder.Append( $"SELECT *, TotalCount = COUNT(*) OVER() FROM {TABLE_PRODUCTS}" );

            AppendCategoryJoin( builder, categoryId );
            AppendSpecJoin( builder, request.CategoryFilters );
            AppendSpecLookupJoin( builder, request.LookupIncludes is not null || request.LookupExcludes is not null );
            
            builder.Append( $" WHERE 1=1" );
            
            AppendCategoryCondition( builder, dynamicParams, categoryId );
            AppendSearchTextCondition( builder, dynamicParams, request.SearchText );
            AppendVendorCondition( builder, dynamicParams, request.Vendors );
            AppendHasSaleCondition( builder, request.HasSale );
            AppendRatingConditions( builder, dynamicParams, request.MinRating );
            AppendPriceConditions( builder, dynamicParams, request.MinPrice, request.MaxPrice );
            AppendSpecConditions( builder, dynamicParams, request.CategoryFilters );
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
    static void AppendCategoryJoin( StringBuilder builder, int? categoryId )
    {
        if ( categoryId is null )
            return;
        
        builder.Append( $" INNER JOIN {TABLE_PRODUCT_CATEGORIES}" );
        builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {TABLE_PRODUCT_CATEGORIES}.{COL_PRODUCT_ID}" );   
    }
    static void AppendSpecJoin( StringBuilder builder, object? filters )
    {
        if ( filters is null )
            return;

        switch ( filters )
        {
            case SearchFiltersBook:
                builder.Append( $" LEFT JOIN {TABLE_PRODUCT_SPECS_BOOK} ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {TABLE_PRODUCT_SPECS_BOOK}.{COL_PRODUCT_ID}" );
                break;
            case SearchFiltersSoftware:
                builder.Append( $" LEFT JOIN {TABLE_PRODUCT_SPECS_SOFTWARE} ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {TABLE_PRODUCT_SPECS_SOFTWARE}.{COL_PRODUCT_ID}" );
                break;
            case SearchFiltersGames:
                builder.Append( $" LEFT JOIN {TABLE_PRODUCT_SPECS_GAMES} ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {TABLE_PRODUCT_SPECS_GAMES}.{COL_PRODUCT_ID}" );
                break;
            case SearchFiltersVideo:
                builder.Append( $" LEFT JOIN {TABLE_PRODUCT_SPECS_MOVIESTV} ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {TABLE_PRODUCT_SPECS_MOVIESTV}.{COL_PRODUCT_ID}" );
                break;
            case SearchFiltersCourses:
                builder.Append( $" LEFT JOIN {TABLE_PRODUCT_SPECS_COURSES} ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {TABLE_PRODUCT_SPECS_COURSES}.{COL_PRODUCT_ID}" );
                break;
            default:
                return;
        }
    }
    static void AppendSpecLookupJoin( StringBuilder builder, bool filtersExist )
    {
        if ( !filtersExist )
            return;
        
        builder.Append( $" INNER JOIN {TABLE_PRODUCT_SPEC_LOOKUPS}" );
        builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {TABLE_PRODUCT_SPEC_LOOKUPS}.{COL_PRODUCT_ID}" );
    }

    // APPEND CONDITIONS
    static void AppendCategoryCondition( StringBuilder builder, DynamicParameters dynamicParams, int? categoryId )
    {
        if ( categoryId is null )
            return;
        
        builder.Append( $" AND ( {TABLE_PRODUCT_CATEGORIES}.{COL_CATEGORY_TIER_ID} = {PARAM_CATEGORY_TYPE}" );
        builder.Append( $" AND {TABLE_PRODUCT_CATEGORIES}.{COL_CATEGORY_ID} = {PARAM_CATEGORY_ID} )" );
        
        dynamicParams.Add( PARAM_CATEGORY_ID, categoryId );
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
    
    static void AppendSpecConditions( StringBuilder builder, DynamicParameters dynamicParams, object? filters ) 
    {
        switch ( filters )
        {
            case SearchFiltersBook book:
                AppendBookConditions( builder, dynamicParams, book );
                break;
            case SearchFiltersSoftware software:
                AppendSoftwareConditions( builder, dynamicParams, software );
                break;
            case SearchFiltersGames games:
                AppendGameConditions( builder, dynamicParams, games );
                break;
            case SearchFiltersVideo video:
                AppendSoftwareConditions( builder, dynamicParams, video );
                break;
            case SearchFiltersCourses course:
                AppendCourseConditions( builder, dynamicParams, course );
                break;
        }
    }
    static void AppendBookConditions( StringBuilder builder, DynamicParameters dynamicParams, SearchFiltersBook? book )
    {
        if ( book is null )
            return;

        if ( book.Pages is not null )
        {
            builder.Append( $" AND ( {TABLE_PRODUCT_SPECS_BOOK}.{COL_PRODUCT_BOOK_PAGES} >= {PARAM_BOOK_MIN_PAGES}" );
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_BOOK}.{COL_PRODUCT_BOOK_PAGES} <= {PARAM_BOOK_MAX_PAGES} )" );
            dynamicParams.Add( PARAM_BOOK_MIN_PAGES, book.Pages.Min );
            dynamicParams.Add( PARAM_BOOK_MAX_PAGES, book.Pages.Max );
        }

        if ( book.HasAudio is not null )
        {
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_BOOK}.{COL_PRODUCT_BOOK_HAS_AUDIO} = {PARAM_BOOK_HAS_AUDIO}" );
            dynamicParams.Add( PARAM_BOOK_HAS_AUDIO, true );
        }
        
        if ( book.AudioLength is not null )
        {
            builder.Append( $" AND ( {TABLE_PRODUCT_SPECS_BOOK}.{COL_PRODUCT_BOOK_AUDIO_LENGTH} >= {PARAM_BOOK_MIN_AUDIO}" );
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_BOOK}.{COL_PRODUCT_BOOK_AUDIO_LENGTH} <= {PARAM_BOOK_MAX_AUDIO} )" );
            dynamicParams.Add( PARAM_BOOK_MIN_AUDIO, book.AudioLength.Min );
            dynamicParams.Add( PARAM_BOOK_MAX_AUDIO, book.AudioLength.Max );
        }
    }
    static void AppendSoftwareConditions( StringBuilder builder, DynamicParameters dynamicParams, SearchFiltersSoftware? software )
    {
        if ( software is null )
            return;

        if ( software.FileSize is not null )
        {
            builder.Append( $" AND ( {TABLE_PRODUCT_SPECS_SOFTWARE}.{COL_PRODUCT_FILESIZE} >= {PARAM_MIN_FILESIZE}" );
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_SOFTWARE}.{COL_PRODUCT_FILESIZE} <= {PARAM_MAX_FILESIZE} )" );
            dynamicParams.Add( PARAM_MIN_FILESIZE, software.FileSize.Min );
            dynamicParams.Add( PARAM_MAX_FILESIZE, software.FileSize.Max );
        }
    }
    static void AppendGameConditions( StringBuilder builder, DynamicParameters dynamicParams, SearchFiltersGames? game )
    {
        if ( game is null )
            return;


        if ( game.HasMultiplayer is not null )
        {
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_GAMES}.{COL_PRODUCT_GAME_HAS_MULTIPLAYER} = {PARAM_GAME_HAS_MULTIPLAYER}" );
            dynamicParams.Add( PARAM_GAME_HAS_MULTIPLAYER, game.HasMultiplayer.Value );
        }
        if ( game.HasOffline is not null )
        {
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_GAMES}.{COL_PRODUCT_GAME_HAS_OFFLINE} = {PARAM_GAME_HAS_OFFLINE}" );
            dynamicParams.Add( PARAM_GAME_HAS_OFFLINE, game.HasOffline.Value );
        }
        if ( game.HasController is not null )
        {
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_GAMES}.{COL_PRODUCT_GAME_HAS_CONTROLLER} = {PARAM_GAME_HAS_CONTROLLER}" );
            dynamicParams.Add( PARAM_GAME_HAS_CONTROLLER, game.HasController.Value );
        }
        if ( game.HasPurchases is not null )
        {
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_GAMES}.{COL_PRODUCT_GAME_HAS_PURCHASES} = {PARAM_GAME_HAS_PURCHASES}" );
            dynamicParams.Add( PARAM_GAME_HAS_PURCHASES, game.HasPurchases.Value );
        }
        if ( game.FileSize is not null )
        {
            builder.Append( $" AND ( {TABLE_PRODUCT_SPECS_GAMES}.{COL_PRODUCT_FILESIZE} >= {PARAM_MIN_FILESIZE}" );
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_GAMES}.{COL_PRODUCT_FILESIZE} <= {PARAM_MAX_FILESIZE} )" );
            dynamicParams.Add( PARAM_MIN_FILESIZE, game.FileSize.Min );
            dynamicParams.Add( PARAM_MAX_FILESIZE, game.FileSize.Max );
        }
    }
    static void AppendSoftwareConditions( StringBuilder builder, DynamicParameters dynamicParams, SearchFiltersVideo? video )
    {
        if ( video is null )
            return;

        if ( video.Runtime is not null )
        {
            builder.Append( $" AND ( {TABLE_PRODUCT_SPECS_MOVIESTV}.{COL_PRODUCT_VIDEO_RUNTIME} >= {PARAM_VIDEO_MIN_RUNTIME}" );
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_MOVIESTV}.{COL_PRODUCT_VIDEO_RUNTIME} <= {PARAM_VIDEO_MAX_RUNTIME} )" );
            dynamicParams.Add( PARAM_VIDEO_MIN_RUNTIME, video.Runtime.Min );
            dynamicParams.Add( PARAM_VIDEO_MAX_RUNTIME, video.Runtime.Max );
        }
        if ( video.Episodes is not null )
        {
            builder.Append( $" AND ( {TABLE_PRODUCT_SPECS_MOVIESTV}.{COL_PRODUCT_VIDEO_EPISODES} >= {PARAM_VIDEO_MIN_EPISODES}" );
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_MOVIESTV}.{COL_PRODUCT_VIDEO_EPISODES} <= {PARAM_VIDEO_MAX_EPISODES} )" );
            dynamicParams.Add( PARAM_VIDEO_MIN_EPISODES, video.Episodes.Min );
            dynamicParams.Add( PARAM_VIDEO_MAX_EPISODES, video.Episodes.Max );
        }

        if ( video.HasSubtitles is not null )
        {
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_MOVIESTV}.{COL_PRODUCT_HAS_SUBTITLES} = {PARAM_HAS_SUBTITLES}" );
            dynamicParams.Add( PARAM_HAS_SUBTITLES, true );
        }
    }
    static void AppendCourseConditions( StringBuilder builder, DynamicParameters dynamicParams, SearchFiltersCourses? course )
    {
        if ( course is null )
            return;

        if ( course.Duration is not null )
        {
            builder.Append( $" AND ( {TABLE_PRODUCT_SPECS_COURSES}.{COL_PRODUCT_COURSE_DURATION} >= {PARAM_COURSE_MIN_DURATION}" );
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_COURSES}.{COL_PRODUCT_COURSE_DURATION} <= {PARAM_COURSE_MAX_DURATION} )" );
            dynamicParams.Add( PARAM_COURSE_MIN_DURATION, course.Duration.Min );
            dynamicParams.Add( PARAM_COURSE_MAX_DURATION, course.Duration.Max );
        }

        if ( course.HasSubtitles is not null )
        {
            builder.Append( $" AND {TABLE_PRODUCT_SPECS_COURSES}.{COL_PRODUCT_HAS_SUBTITLES} = {PARAM_HAS_SUBTITLES}" );
            dynamicParams.Add( PARAM_HAS_SUBTITLES, true );
        }
    }
    
    sealed class SearchQueryObject
    {
        public string SearchQuery { get; init; } = string.Empty;
        public DynamicParameters DynamicParams { get; init; } = new();
    }
}