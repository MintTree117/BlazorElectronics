using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories;

public abstract class DapperRepository
{
    // TABLES PRODUCTS
    protected const string TABLE_PRODUCTS = "Products";
    protected const string TABLE_PRODUCT_CATEGORIES = "Product_Categories";
    protected const string TABLE_PRODUCT_DESCRIPTIONS = "Product_Descriptions";
    protected const string TABLE_PRODUCT_IMAGES = "Product_Images";
    protected const string TABLE_PRODUCT_REVIEWS = "Product_Reviews";
    protected const string TABLE_PRODUCT_VARIANTS = "Product_Variants";
    protected const string TABLE_PRODUCT_SPECS_INT_FILTERS = "Product_Specs_Explicit_Int_Filters";
    protected const string TABLE_PRODUCT_SPECS_INT = "Product_Specs_Explicit_Int_Values";
    protected const string TABLE_PRODUCT_SPECS_STRING = "Product_Specs_Explicit_String_Values";
    protected const string TABLE_PRODUCT_SPECS_BOOL = "Specs_Explicit_Bool";

    // TABLES PRODUCT SPECS MAIN
    protected enum ProductSpecMainTableNameEnum
    {
        BOOKS, SOFTWARE, GAMES, TVMOVIES, COURSES
    }
    const string TABLE_PATTERN_PRODUCT_SPECS_MAIN = "Product_Specs_Main";
    protected static readonly Dictionary<ProductSpecMainTableNameEnum, string> PRODUCT_SPECS_MAIN_TABLE_NAMES = new()
    {
        { ProductSpecMainTableNameEnum.BOOKS, TABLE_PATTERN_PRODUCT_SPECS_MAIN + "_Books" },
        { ProductSpecMainTableNameEnum.SOFTWARE, TABLE_PATTERN_PRODUCT_SPECS_MAIN + "_Software" },
        { ProductSpecMainTableNameEnum.GAMES, TABLE_PATTERN_PRODUCT_SPECS_MAIN + "_Games" },
        { ProductSpecMainTableNameEnum.TVMOVIES, TABLE_PATTERN_PRODUCT_SPECS_MAIN + "_TvMovies" },
        { ProductSpecMainTableNameEnum.COURSES, TABLE_PATTERN_PRODUCT_SPECS_MAIN + "_Courses" },
    };

    // COLUMNS CATEGORIES
    protected const string COL_CATEGORY_PRIMARY_ID = "PrimaryCategoryId";
    protected const string COL_CATEGORY_SECONDARY_ID = "SecondaryCategoryId";
    protected const string COL_CATEGORY_TERTIARY_ID = "TertiaryCategoryId";
    protected const string COL_CATEGORY_ID = "CategoryId";
    protected const string COL_CATEGORY_TIER_ID = "CategoryTierId";
    
    // COLUMNS PRODUCTS
    protected const string COL_PRODUCT_ID = "ProductId";
    protected const string COL_PRODUCT_TITLE = "Title";
    protected const string COL_PRODUCT_RATING = "Rating";
    protected const string COL_PRODUCT_IMAGE_ID = "ImageId";
    protected const string COL_PRODUCT_REVIEW_ID = "ReviewId";
    protected const string COL_PRODUCT_THUMBNAIL = "Thumbnail";
    protected const string COL_PRODUCT_HAS_SALE = "HasSale";
    protected const string COL_PRODUCT_DESCRIPTION_ID_COLUMN = "DescriptionId";
    protected const string COL_PRODUCT_DESCR_BODY = "DescriptionBody";
    protected const string COL_PRODUCT_LOWEST_PRICE = "ProductLowestPrice";
    protected const string COL_PRODUCT_HIGHEST_PRICE = "ProductHighestPrice";
    protected const string COL_PRODUCT_LAST_UPDATED = "LastUpdated";
    protected const string COL_PRODUCT_FILE_SIZE = "FileSize";
    protected const string COL_PRODUCT_HAS_SUBTITLES = "HasSubtitles";

    protected const string COL_SPEC_ID = "SpecId";
    protected const string COL_FILTER_INT_ID = "FilterId";
    protected const string COL_SPEC_VALUE_ID = "SpecValueId";

    // COLUMNS PRODUCT VARIANTS
    protected const string COL_VARIANT_ID = "VariantId";
    protected const string COL_VARIANT_SUB_ID = "VariantSubId";
    protected const string COL_VARIANT_NAME = "Name";
    protected const string COL_VARIANT_PRICE_ORIGINAL = "OriginalPrice";
    protected const string COL_VARIANT_PRICE_SALE = "SalePrice";
    
    // COLUMNS BOOKS
    protected const string COL_BOOKS_PUBLISHER_ID = "PublisherId";
    protected const string COL_BOOKS_AUTHOR_ID = "AuthorId";
    protected const string COL_BOOKS_PAGES = "Pages";
    protected const string COL_BOOKS_ACCESSIBILITY = "HasAccessibility";
    protected const string COL_BOOKS_HAS_AUDIO = "HasAudio";
    protected const string COL_BOOKS_AUDIO_LENGTH = "AudioLength";
    
    // COLUMNS SOFTWARE
    protected const string COL_SOFTWARE_DEVELOPER_ID = "PublisherId";
    
    // COLUMNS GAMES
    protected const string COL_GAMES_GAME_DEVELOPER_ID = "GameDeveloperId";
    protected const string COL_GAMES_ESRB_RATING = "EsrbRating";
    protected const string COL_GAMES_HAS_MULTIPLAYER = "HasMultiplayer";
    protected const string COL_GAMES_HAS_IN_GAME_PURCHASES = "HasInGamePurchases";
    protected const string COL_GAMES_HAS_CONTROLLER_SUPPORT = "HasControllerSupport";
    
    // COLUMNS MOVIES_TV
    protected const string COL_MOVIESTV_IS_TV = "IsTvShow";
    protected const string COL_MOVIESTV_IS_MOVIE = "IsMovie";
    protected const string COL_MOVIESTV_RUNTIME = "Runtime";
    protected const string COL_MOVIESTV_EPISODES = "Episodes";
    
    // COLUMNS COURSES
    protected const string COL_COURSES_LECTURES = "NumLectures";
    protected const string COL_COURSES_EFFORT = "Effor";
    protected const string COL_COURSES_DURATION = "Duration";
    protected const string COL_COURSES_ACCREDATION = "Accredation";
    protected const string COL_COURSES_CERTIFICATE = "Certificate";
    
    // COLUMNS USERS
    protected const string COL_USER_NAME = "Username";
    protected const string COL_USER_EMAIL = "Email";
    protected const string COL_USER_HASH = "PasswordHash";
    protected const string COL_USER_SALT = "PasswordSalt";
    protected const string COL_USER_DATE = "DateCreated";
    
    // EXCEPTION CONSTS
    const int MAX_RETRIES = 3;
    const int RETRY_DELAY_MILLISECONDS = 1000;
    
    // DB CONTEXT
    protected readonly DapperContext _dbContext;

    protected DapperRepository( DapperContext dapperContext )
    {
        _dbContext = dapperContext;
    }

    protected delegate Task<T?> DapperQueryDelegate<T>( SqlConnection connection, string? dynamicSql = null, DynamicParameters? dynamicParams = null );
    protected delegate Task<T?> DapperQueryTransactionDelegate<T>( SqlConnection connection, DbTransaction transaction, string? dynamicSql = null, DynamicParameters? dynamicParams = null );
    
    protected async Task<T?> TryQueryAsync<T>( DapperQueryDelegate<T> dapperQueryDelegate, DynamicParameters? dynamicParams = null, string? dynamicSql = null )
    {
        int currentRetry = 0;
        
        while ( currentRetry < MAX_RETRIES )
        {
            try
            {
                await using SqlConnection? connection = await _dbContext.GetOpenConnection();
                return await dapperQueryDelegate.Invoke( connection, dynamicSql, dynamicParams );
            }
            catch ( Exception ex ) when ( ex is TimeoutException )
            {
                currentRetry++;
                await Task.Delay( RETRY_DELAY_MILLISECONDS );
            }
        }

        return default;
    }
    protected async Task<T?> TryQueryTransactionAsync<T>( DapperQueryTransactionDelegate<T> dapperQueryDelegate, DynamicParameters? dynamicParams = null, string? dynamicSql = null )
    {
        SqlConnection connection = null;
        DbTransaction transaction = null;
        
        int currentConnectionRetry = 0;
        int currentQueryRetry = 0;

        while ( currentConnectionRetry < MAX_RETRIES )
        {
            try
            {
                connection = await _dbContext.GetOpenConnection();
                transaction = await connection.BeginTransactionAsync();
            }
            catch ( Exception ex ) when ( ex is TimeoutException )
            {
                currentConnectionRetry++;
                await Task.Delay( RETRY_DELAY_MILLISECONDS );

                if ( currentConnectionRetry <= MAX_RETRIES ) 
                    continue;
                
                await HandleConnectionTransactionDisposal( connection, transaction );
                return default;
            }
        }

        while ( currentQueryRetry < MAX_RETRIES )
        {
            try
            {
                T? transactionResult = await dapperQueryDelegate.Invoke( connection!, transaction!, dynamicSql, dynamicParams );
                await transaction!.CommitAsync();
                await transaction.DisposeAsync();
                await connection!.CloseAsync();
                return transactionResult;
            }
            catch ( Exception ex ) when ( ex is TimeoutException )
            {
                currentQueryRetry++;
                await Task.Delay( RETRY_DELAY_MILLISECONDS );

                if ( currentQueryRetry <= MAX_RETRIES )
                    continue;

                await HandleConnectionTransactionRollbackDisposal( connection, transaction );
                return default;
            }
        }
        
        return default;
    }

    static async Task HandleConnectionTransactionDisposal( SqlConnection? connection, DbTransaction? transaction = null )
    {
        if ( transaction != null )
            await transaction.DisposeAsync();
        if ( connection != null )
            await connection.CloseAsync();
    }
    static async Task HandleConnectionTransactionRollbackDisposal( SqlConnection? connection, DbTransaction? transaction = null )
    {
        if ( transaction != null )
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();
        }
        if ( connection != null )
            await connection.CloseAsync();
    }
}