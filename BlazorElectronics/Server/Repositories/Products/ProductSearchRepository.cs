using System.Data;
using System.Text;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Repositories.Products;

public sealed class ProductSearchRepository : DapperRepository, IProductSearchRepository
{
    // QUERY PARAM NAMES
    const string PARAM_PRODUCT_ID = "@ProductId";
    const string PARAM_MIN_RATING = "@minRating";
    const string PARAM_MAX_RATING = "@maxRating";
    const string PARAM_MIN_PRICE = "@minPrice";
    const string PARAM_MAX_PRICE = "@maxPrice";
    const string PARAM_SPEC_FILTER_LOOKUP = "@lookupSpecFilter";
    const string PARAM_SPEC_FILTER_RAW = "@rawSpecFilter";
    const string PARAM_QUERY_OFFSET = "@queryOffset";
    const string PARAM_QUERY_ROWS = "@queryRows";
    const string PARAM_SEARCH_TEXT = "@searchText";
    const string PARAM_CATEGORY_TIER = "@CategoryTier";
    const string PARAM_CATEGORY_ID = "@CategoryId";

    const string PARAM_VARIANT_ID = "@VariantId";
    const string PARAM_LANGUAGE_ID = "@LanguageId";
    const string PARAM_CONTENTFLAG_ID = "@ContentFlagId";
    const string PARAM_MEDIAFORMAT_ID = "@MediaFormatId";
    const string PARAM_HAS_SUBTITLES = "@HasSubtitles";
    const string PARAM_SUBTITLE_LANGUAGE_ID = "@SubtitleLanguageId";

    const string PARAM_BOOK_PUBLISHER_ID = "@PublisherId";
    const string PARAM_BOOK_AUTHOR_ID = "@AuthorId";
    const string PARAM_BOOK_MIN_PAGES = "@MinPages";
    const string PARAM_BOOK_MAX_PAGES = "@MaxPages";
    const string PARAM_BOOK_HAS_ACCESSIBILITY = "@HasAccessibility";
    const string PARAM_BOOK_HAS_AUDIO = "@HasAudio";
    const string PARAM_BOOK_MIN_AUDIO_LENGTH = "@MinAudioLength";
    const string PARAM_BOOK_MAX_AUDIO_LENGTH = "@MaxAudioLength";

    const string PARAM_SOFTWARE_DEVELOPER_ID = "@SoftwareDeveloperId";

    const string PARAM_COURSE_MIN_LECTURES = "@MinNumLectures";
    const string PARAM_COURSE_MAX_LECTURES = "@MaxNumLectures";
    const string PARAM_COURSE_MIN_EFFORT = "@MinHoursPerWeek";
    const string PARAM_COURSE_MAX_EFFORT = "@MxHoursPerWeek";
    const string PARAM_COURSE_MIN_DURATION = "@MinCourseDuration";
    const string PARAM_COURSE_MAX_DURATION = "@MaxCourseDuration";
    const string PARAM_COURSE_ACCREDATION = "@CourseAccredation";
    const string PARAM_COURSE_CERTIIFICATION = "@CourseCertification";
    
    const string XML_VARIANT_DATA_ROOT = "Variants";
    const string XML_VARIANT_DATA = "Variant";

    const string CTE_FILTERED_PRODUCTS = "FilteredProducts_CTE";
    const string CTE_PRODUCT_VARIANTS = "ProductVariants_CTE";
    
    // STORED PROCEDURES
    const string STORED_PROCEDURE_GET_NAMES_BY_SEARCH_TEXT = "Get_ProductSearchSuggestions";
    const string STORED_PROCEDURE_GET_ALL_PRODUCTS = "Get_AllProducts";
    const string STORED_PROCEDURE_GET_PRODUCT_BY_ID = "Get_ProductById";
    
    public ProductSearchRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<string?> GetProductSearchQueryString( ProductSearchRequest searchRequest )
    {
        var productQuery = new StringBuilder();
        var countQuery = new StringBuilder();

        //await BuildProductSearchQuery( searchRequest, productQuery, countQuery );
        return productQuery + "-----------------------------------------" + countQuery;
    }
    
    public async Task<IEnumerable<string>?> GetSearchSuggestions( string searchText, int categoryTier, int categoryId )
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
    public async Task<ProductSearch?> GetProductSearch( CategoryIdMap categoryIdMap, ProductSearchRequest searchRequest )
    {
        var productQuery = new StringBuilder();
        var countQuery = new StringBuilder();

        DynamicParameters dynamicParams = new DynamicParameters(); // await BuildProductSearchQuery( searchRequest, productQuery, countQuery );

        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();

            Task<IEnumerable<Product>?> productTask = ExecuteProductSearch( connection, productQuery.ToString(), dynamicParams );
            Task<int> countTask = ExecuteSearchCount( connection, countQuery.ToString(), dynamicParams );

            await Task.WhenAll( productTask, countTask );

            if ( productTask.Result == null )
                return null;

            return new ProductSearch {
                Products = productTask.Result,
                TotalSearchCount = countTask.Result,
                QueryRows = dynamicParams.Get<int>( PARAM_QUERY_ROWS ),
                QueryOffset = dynamicParams.Get<int>( PARAM_QUERY_OFFSET )
            };
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
    
    static async Task<int> ExecuteSearchCount( SqlConnection? connection, string dynamicQuery, DynamicParameters dynamicParams )
    {
        try
        {
            return await connection.QueryFirstAsync<int>( dynamicQuery, dynamicParams, commandType: CommandType.Text );
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
    static async Task<IEnumerable<Product>?> ExecuteProductSearch( SqlConnection? connection, string dynamicQuery, DynamicParameters dynamicParams )
    {
        var productDictionary = new Dictionary<int, Product>();

        try
        {
            await connection.QueryAsync<Product, string, Product>
            ( dynamicQuery, ( product, variantXml ) =>
                {
                    if ( !productDictionary.TryGetValue( product.ProductId, out Product? productEntry ) )
                    {
                        productEntry = product;
                        productDictionary.Add( productEntry.ProductId, productEntry );
                    }
                    if ( variantXml != null )
                        productEntry.ProductVariants = ParseVariantData( variantXml );
                    return productEntry;
                },
                dynamicParams,
                splitOn: XML_VARIANT_DATA,
                commandType: CommandType.Text );

            return productDictionary.Values;
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
    static List<ProductVariant> ParseVariantData( string variantXml )
    {
        if ( string.IsNullOrEmpty( variantXml ) )
            return new List<ProductVariant>();

        XDocument doc = XDocument.Parse( variantXml );

        if ( doc.Root == null )
            return new List<ProductVariant>();

        var variantsList = new List<ProductVariant>();

        foreach ( XElement e in doc.Root.Elements( XML_VARIANT_DATA ) )
        {
            var variant = new ProductVariant();

            if ( e.Element( COL_VARIANT_SUB_ID ) == null ||
                 e.Element( COL_VARIANT_NAME ) == null ||
                 e.Element( COL_VARIANT_PRICE_ORIGINAL ) == null ||
                 e.Element( COL_VARIANT_PRICE_SALE ) == null )
            {
                variant.VariantName = "NULL VARIANT DATA!";
                variantsList.Add( variant );
                continue;
            }

            variant.VariantId = ( int ) e.Element( COL_VARIANT_SUB_ID )!;
            variant.VariantName = ( string ) e.Element( COL_VARIANT_NAME )!;
            variant.VariantPriceMain = ( decimal ) e.Element( COL_VARIANT_PRICE_ORIGINAL )!;
            variant.VariantPriceSale = ( decimal ) e.Element( COL_VARIANT_PRICE_SALE )!;

            variantsList.Add( variant );
        }

        return variantsList;
    }

    static async Task<SearchQueryObject> BuildProductSearchQueryObject( CategoryIdMap categoryMap, ProductSearchRequest request, Dictionary<int, DynamicTableInfo> dynamicTableInfo )
    {
        var queryObject = new SearchQueryObject();
        
        await Task.Run( () =>
        {
            queryObject = BuildBaseSearchSelectionObject( categoryMap, request, dynamicTableInfo );

            queryObject = ( request switch {
                ProductSearchRequestBooks books => BuildSubSearchQuerySelection( ProductSpecMainTableNameEnum.BOOKS, queryObject, dynamicTableInfo, books.BooksDynamicFiltersInclude, books.BooksDynamicFiltersExclude ),
                ProductSearchRequestSoftware software => BuildSubSearchQuerySelection( ProductSpecMainTableNameEnum.SOFTWARE, queryObject, dynamicTableInfo, software.SoftwareDynamicFiltersInclude, software.SoftwareDynamicFiltersExclude ),
                ProductSearchRequestGames games => BuildSubSearchQuerySelection( ProductSpecMainTableNameEnum.GAMES, queryObject, dynamicTableInfo, games.GamesDynamicFiltersInclude, games.GamesDynamicFiltersExclude ),
                ProductSearchRequestMoviesTv tv => BuildSubSearchQuerySelection( ProductSpecMainTableNameEnum.TVMOVIES, queryObject, dynamicTableInfo, tv.MoviesTvDynamicFiltersInclude, tv.MoviesTvDynamicFiltersExclude ),
                ProductSearchRequestCourses courses => BuildSubSearchQuerySelection( ProductSpecMainTableNameEnum.COURSES, queryObject, dynamicTableInfo, courses.CoursesDynamicFiltersInclude, courses.CoursesDynamicFiltersExclude ),
                _ => null
            } )!;

            /*queryObject = BuildBaseSearchQueryFilteringObject( queryObject, request );

            queryObject = ( request switch {
                ProductSearchRequestBooks books => BuildBookSearchQueryFilteringObject( queryObject, books ),
                ProductSearchRequestSoftware software => BuildSoftwareSearchQueryFilteringObject( queryObject, software ),
                ProductSearchRequestGames games => BuildGamesSearchQueryFilteringObject( queryObject, games ),
                ProductSearchRequestMoviesTv tv => BuildMoviesTvSearchQueryFilteringObject( queryObject, tv ),
                ProductSearchRequestCourses courses => BuildCoursesSearchQueryFilteringObject( queryObject, courses ),
                _ => null
            } )!;*/
        } );

        return queryObject;
    }

    static SearchQueryObject BuildBaseSearchSelectionObject( CategoryIdMap categoryMap, ProductSearchRequest request, Dictionary<int, DynamicTableInfo> dynamicTableInfo )
    {
        var searchBuilder = new StringBuilder();
        var queryObject = new SearchQueryObject {
            SearchQueryBuilder = searchBuilder
        };
        
        // SELECT PRODUCTS & PRODUCT_CATEGORIES
        searchBuilder.Append( $"SELECT * FROM {TABLE_PRODUCTS}" );
        searchBuilder.Append( $" INNER JOIN {TABLE_PRODUCT_CATEGORIES}" );
        searchBuilder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {TABLE_PRODUCT_CATEGORIES}.{COL_PRODUCT_ID}" );

        // DYNAMIC JOINS
        if ( request.GlobalDynamicFiltersInclude is not null )
        {
            AppendDynamicQueryJoins( searchBuilder, request.GlobalDynamicFiltersInclude, dynamicTableInfo );
        }
        if ( request.GlobalDynamicFiltersExclude is not null )
        {
            AppendDynamicQueryJoins( searchBuilder, request.GlobalDynamicFiltersExclude, dynamicTableInfo );
        }

        return queryObject;
    }

    static SearchQueryObject BuildBaseSearchQueryFilteringObject( SearchQueryObject queryObject, ProductSearchRequest request, Dictionary<int, DynamicTableInfo> dynamicTableInfo )
    {
        StringBuilder builder = queryObject.SearchQueryBuilder;
        DynamicParameters dynamicParams = queryObject.DynamicParams;
        
        // FILTER CATEGORIES
        builder.Append( $" WHERE {TABLE_PRODUCT_CATEGORIES}.{COL_CATEGORY_TIER_ID} = {PARAM_CATEGORY_TIER}" );
        builder.Append( $" AND {TABLE_PRODUCT_CATEGORIES}.{COL_CATEGORY_ID} = {PARAM_CATEGORY_ID}" );
        
        // FILTER SALES
        if ( request.HasSale.HasValue && request.HasSale.Value )
        {
            builder.Append( $" AND ( {TABLE_PRODUCTS}.{COL_PRODUCT_HAS_SALE} = 1" );
        }
        
        // FILTER SEARCH TEXT
        if ( !string.IsNullOrWhiteSpace( request.SearchText ) )
        {
            builder.Append( $" AND ( {TABLE_PRODUCTS}.{COL_PRODUCT_TITLE} LIKE {request.SearchText}" );
            builder.Append( $" OR ( {TABLE_PRODUCT_DESCRIPTIONS}.{COL_PRODUCT_DESCR_BODY} LIKE {request.SearchText} )" );
            dynamicParams.Add( PARAM_SEARCH_TEXT, request.SearchText );
        }
        
        // FILTER RATING
        if ( request.MinRating.HasValue )
        {
            builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_RATING} > {PARAM_MIN_RATING}" );
            dynamicParams.Add( PARAM_MIN_RATING, request.MinRating.Value );
        }
        if ( request.MaxRating.HasValue )
        {
            builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_RATING} < {PARAM_MAX_RATING}" );
            dynamicParams.Add( PARAM_MAX_RATING, request.MaxRating.Value );
        }
        
        // FILTER PRICE
        if ( request.MinPrice.HasValue )
        {
            builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_LOWEST_PRICE} > {PARAM_MIN_PRICE}" );
            dynamicParams.Add( PARAM_MIN_RATING, request.MinPrice.Value );
        }
        if ( request.MaxPrice.HasValue )
        {
            builder.Append( $" AND {TABLE_PRODUCTS}.{COL_PRODUCT_HIGHEST_PRICE} < {PARAM_MAX_PRICE}" );
            dynamicParams.Add( PARAM_MAX_RATING, request.MaxPrice.Value );
        }

        return BuildSubDynamicSearchQueryConditions( queryObject, request.GlobalDynamicFiltersInclude, request.GlobalDynamicFiltersExclude, dynamicTableInfo, dynamicParams );
    }
    static SearchQueryObject BuildBookSearchQueryFilteringObject( SearchQueryObject queryObject, ProductSearchRequestBooks booksRequest, Dictionary<int, DynamicTableInfo> dynamicTableInfo )
    {
        StringBuilder builder = queryObject.SearchQueryBuilder;
        DynamicParameters dynamicParams = queryObject.DynamicParams;
        string booksTable = PRODUCT_SPECS_MAIN_TABLE_NAMES[ ProductSpecMainTableNameEnum.BOOKS ];
        
        // PUBLISHER & AUTHOR
        if ( booksRequest.PublisherId.HasValue )
        {
            builder.Append( $" AND {booksTable}.{COL_BOOKS_PUBLISHER_ID} = {PARAM_BOOK_PUBLISHER_ID}" );
            dynamicParams.Add( PARAM_BOOK_PUBLISHER_ID, booksRequest.PublisherId.Value );
        }
        if ( booksRequest.AuthorId.HasValue )
        {
            builder.Append( $" AND {booksTable}.{COL_BOOKS_AUTHOR_ID} = {PARAM_BOOK_AUTHOR_ID}" );
            dynamicParams.Add( PARAM_BOOK_AUTHOR_ID, booksRequest.AuthorId.Value );
        }
        
        // PAGES
        if ( booksRequest.MinPages.HasValue )
        {
            builder.Append( $" AND {booksTable}.{COL_BOOKS_PAGES} >= {PARAM_BOOK_MIN_PAGES}" );
            dynamicParams.Add( PARAM_BOOK_MIN_PAGES, booksRequest.MinPages.Value );
        }
        if ( booksRequest.MaxPages.HasValue )
        {
            builder.Append( $" AND {booksTable}.{COL_BOOKS_PAGES} <= {PARAM_BOOK_MAX_PAGES}" );
            dynamicParams.Add( PARAM_BOOK_MAX_PAGES, booksRequest.MaxPages.Value );
        }
        
        // ACCESSIBILITY
        if ( booksRequest.HasAccessibility.HasValue && booksRequest.HasAccessibility.Value )
        {
            builder.Append( $" AND {booksTable}.{COL_BOOKS_ACCESSIBILITY} = {PARAM_BOOK_HAS_ACCESSIBILITY}" );
            dynamicParams.Add( PARAM_BOOK_HAS_ACCESSIBILITY, 1 );
        }
        
        // AUDIO
        if ( booksRequest.HasAudio.HasValue )
        {
            builder.Append( $" AND {booksTable}.{COL_BOOKS_HAS_AUDIO} = {PARAM_BOOK_HAS_AUDIO}" );
            dynamicParams.Add( PARAM_BOOK_HAS_AUDIO, booksRequest.HasAudio.Value ? 1 : 0 );

            if ( booksRequest.MinAudioLength.HasValue )
            {
                builder.Append( $" AND {booksTable}.{COL_BOOKS_AUDIO_LENGTH} >= {PARAM_BOOK_MIN_AUDIO_LENGTH}" );
                dynamicParams.Add( PARAM_BOOK_MIN_AUDIO_LENGTH, booksRequest.MinAudioLength );
            }

            if ( booksRequest.MaxAudioLength.HasValue )
            {
                builder.Append( $" AND {booksTable}.{COL_BOOKS_AUDIO_LENGTH} = {PARAM_BOOK_MAX_AUDIO_LENGTH}" );
                dynamicParams.Add( PARAM_BOOK_MAX_AUDIO_LENGTH, booksRequest.MaxAudioLength );
            }
        }

        return BuildSubDynamicSearchQueryConditions( queryObject, booksRequest.BooksDynamicFiltersInclude, booksRequest.BooksDynamicFiltersExclude, dynamicTableInfo, dynamicParams );
    }
    static SearchQueryObject BuildSoftwareSearchQueryFilteringObject( SearchQueryObject queryObject, ProductSearchRequestSoftware softwareRequest, Dictionary<int, DynamicTableInfo> dynamicTableInfo )
    {
        StringBuilder builder = queryObject.SearchQueryBuilder;
        DynamicParameters dynamicParams = queryObject.DynamicParams;
        string softwareTable = PRODUCT_SPECS_MAIN_TABLE_NAMES[ ProductSpecMainTableNameEnum.SOFTWARE ];

        if ( softwareRequest.SoftwareDeveloperId.HasValue )
        {
            builder.Append( $" AND {softwareTable}.{COL_SOFTWARE_DEVELOPER_ID} = {PARAM_SOFTWARE_DEVELOPER_ID}" );
            dynamicParams.Add( PARAM_SOFTWARE_DEVELOPER_ID, softwareRequest.SoftwareDeveloperId.Value );
        }

        return BuildSubDynamicSearchQueryConditions( queryObject, softwareRequest.SoftwareDynamicFiltersInclude, softwareRequest.SoftwareDynamicFiltersExclude, dynamicTableInfo, dynamicParams );
    }
    static SearchQueryObject BuildGamesSearchQueryFilteringObject( SearchQueryObject queryObject, ProductSearchRequestGames gamesRequest, Dictionary<int, DynamicTableInfo> dynamicTableInfo )
    {
        StringBuilder builder = queryObject.SearchQueryBuilder;
        DynamicParameters dynamicParams = queryObject.DynamicParams;
        string gamesTable = PRODUCT_SPECS_MAIN_TABLE_NAMES[ ProductSpecMainTableNameEnum.GAMES ];
        
        return BuildSubDynamicSearchQueryConditions( queryObject, gamesRequest.GlobalDynamicFiltersInclude, gamesRequest.GamesDynamicFiltersExclude, dynamicTableInfo, dynamicParams );
    }
    static SearchQueryObject BuildMoviesTvSearchQueryFilteringObject( SearchQueryObject queryObject, ProductSearchRequestMoviesTv moviesTvRequest, Dictionary<int, DynamicTableInfo> dynamicTableInfo )
    {
        StringBuilder builder = queryObject.SearchQueryBuilder;
        DynamicParameters dynamicParams = queryObject.DynamicParams;
        string moviesTvTable = PRODUCT_SPECS_MAIN_TABLE_NAMES[ ProductSpecMainTableNameEnum.TVMOVIES ];

        return BuildSubDynamicSearchQueryConditions( queryObject, moviesTvRequest.MoviesTvDynamicFiltersInclude, moviesTvRequest.MoviesTvDynamicFiltersExclude, dynamicTableInfo, dynamicParams );
    }
    static SearchQueryObject BuildCoursesSearchQueryFilteringObject( SearchQueryObject queryObject, ProductSearchRequestCourses coursesRequest, Dictionary<int, DynamicTableInfo> dynamicTableInfo )
    {
        StringBuilder builder = queryObject.SearchQueryBuilder;
        DynamicParameters dynamicParams = queryObject.DynamicParams;
        string coursesTable = PRODUCT_SPECS_MAIN_TABLE_NAMES[ ProductSpecMainTableNameEnum.TVMOVIES ];

        return BuildSubDynamicSearchQueryConditions( queryObject, coursesRequest.CoursesDynamicFiltersInclude, coursesRequest.CoursesDynamicFiltersExclude, dynamicTableInfo, dynamicParams );
    }

    static SearchQueryObject BuildSubSearchQuerySelection( ProductSpecMainTableNameEnum nameEnum, SearchQueryObject queryObject, Dictionary<int, DynamicTableInfo> dynamicTableInfo, Dictionary<int, List<int>>? includes, Dictionary<int, List<int>>? excludes )
    {
        StringBuilder builder = queryObject.SearchQueryBuilder;
        string tableName = PRODUCT_SPECS_MAIN_TABLE_NAMES[ nameEnum ];

        builder.Append( $" INNER JOIN {tableName}" );
        builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {tableName}.{COL_PRODUCT_ID}" );

        // FILTER DYNAMIC INCLUDE
        if ( includes is not null )
        {
            AppendDynamicQueryJoins( builder, includes, dynamicTableInfo );
        }
        // FILTER DYNAMIC INCLUDE
        if ( excludes is not null )
        {
            AppendDynamicQueryJoins( builder, excludes, dynamicTableInfo );
        }

        return queryObject;
    }
    static SearchQueryObject BuildSubDynamicSearchQueryConditions( SearchQueryObject queryObject, Dictionary<int, List<int>>? filtersInclude, Dictionary<int, List<int>>? filtersExlude, Dictionary<int, DynamicTableInfo> dynamicTableInfo, DynamicParameters dynamicParams )
    {
        if ( filtersInclude is not null )
        {
            AppendDynamicQueryConditions( queryObject.SearchQueryBuilder, filtersInclude, dynamicTableInfo, dynamicParams, true );
        }
        
        if ( filtersExlude is not null )
        {
            AppendDynamicQueryConditions( queryObject.SearchQueryBuilder, filtersExlude, dynamicTableInfo, dynamicParams, false );
        }

        return queryObject;
    }
    
    static void AppendDynamicQueryJoins( StringBuilder builder, Dictionary<int, List<int>> tableIds, Dictionary<int, DynamicTableInfo> dynamicTableInfo )
    {
        if ( tableIds?.Any() != true )
            return;

        foreach ( int tableId in tableIds.Keys )
        {
            string tableName = dynamicTableInfo[ tableId ].TableName;

            builder.Append( $" INNER JOIN {tableName}" );
            builder.Append( $" ON {TABLE_PRODUCTS}.{COL_PRODUCT_ID} = {tableName}.{COL_PRODUCT_ID}" );
        }
    }
    static void AppendDynamicQueryConditions( StringBuilder builder, Dictionary<int, List<int>> filtersInclude, Dictionary<int, DynamicTableInfo> dynamicTableInfo, DynamicParameters dynamicParams, bool include )
    {
        if ( filtersInclude?.Any() != true ) 
            return;
        
        foreach ( KeyValuePair<int, List<int>> kvp in filtersInclude )
        {
            int tableId = kvp.Key;
            List<int> filterValues = kvp.Value;

            if ( !dynamicTableInfo.TryGetValue( tableId, out DynamicTableInfo? tableInfo ) )
                continue;
            
            var parameterNames = new List<string>();
            
            for ( int i = 0; i < filterValues.Count; i++ )
            {
                string paramName = $"@{tableInfo.IdColumnName}{i}";
                parameterNames.Add( $"{tableInfo.TableName}.{tableInfo.IdColumnName} = {paramName}" );
                dynamicParams.Add( paramName, filterValues[ i ] );
            }

            string joinedParameters = string.Join( " OR ", parameterNames );
            string condition = include ? $" AND ( {joinedParameters} )" : $" AND NOT ( {joinedParameters} )";
            builder.Append( condition );
        }
    }

    static Dictionary<int, string> GenerateTableQueryNames()
    {
        return new Dictionary<int, string>();
    }
    static Dictionary<int, string> GenerateIdParamNames()
    {
        return new Dictionary<int, string>();
    }
    
    class SearchQueryObject
    {
        public StringBuilder SearchQueryBuilder { get; set; } = new();
        public DynamicParameters DynamicParams { get; set; } = new();
    }
    class DynamicTableInfo
    {
        public string TableName { get; set; } = string.Empty;
        public string IdColumnName { get; set; } = string.Empty;
    }
}