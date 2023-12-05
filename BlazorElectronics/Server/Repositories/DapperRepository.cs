using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Enums;
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
    protected const string TABLE_PRODUCT_SPEC_LOOKUPS = "Product_Specs_Lookup";
    protected const string TABLE_PRODUCT_SPECS_BOOK = "Product_Specs_Book";
    protected const string TABLE_PRODUCT_SPECS_SOFTWARE = "Product_Specs_Software";
    protected const string TABLE_PRODUCT_SPECS_GAMES = "Product_Specs_Games";
    protected const string TABLE_PRODUCT_SPECS_MOVIESTV = "Product_Specs_MoviesTV";
    protected const string TABLE_PRODUCT_SPECS_COURSES = "Product_Specs_Courses";

    // COLUMNS CATEGORIES
    protected const string COL_CATEGORY_ID = "CategoryId";
    protected const string COL_CATEGORY_TIER = "CategoryTier";
    
    // COLUMNS PRODUCTS
    protected const string COL_PRODUCT_ID = "ProductId";
    protected const string COL_PRODUCT_TITLE = "Title";
    protected const string COL_PRODUCT_RATING = "Rating";
    protected const string COL_PRODUCT_THUMBNAIL = "Thumbnail";
    protected const string COL_PRODUCT_DESCR = "Description";
    protected const string COL_PRODUCT_PRICE = "Price";
    protected const string COL_PRODUCT_SALE_PRICE = "SalePrice";
    // SHARED SPECS
    protected const string COL_PRODUCT_LAST_UPDATED = "LastUpdated";
    protected const string COL_PRODUCT_FILESIZE = "FileSize";
    protected const string COL_PRODUCT_HAS_SUBTITLES = "HasSubtitles";
    // BOOKS
    protected const string COL_PRODUCT_BOOK_PAGES = "Pages";
    protected const string COL_PRODUCT_BOOK_HAS_AUDIO = "HasAudio";
    protected const string COL_PRODUCT_BOOK_AUDIO_LENGTH = "AudioLength";
    // BOOKS
    protected const string COL_PRODUCT_GAME_HAS_MULTIPLAYER = "HasMultiplayer";
    protected const string COL_PRODUCT_GAME_HAS_OFFLINE = "HasOffline";
    protected const string COL_PRODUCT_GAME_HAS_CONTROLLER = "HasController";
    protected const string COL_PRODUCT_GAME_HAS_PURCHASES = "HasPurchases";
    // VIDEO
    protected const string COL_PRODUCT_VIDEO_RUNTIME = "Runtime";
    protected const string COL_PRODUCT_VIDEO_EPISODES = "Episodes";
    // COURSES
    protected const string COL_PRODUCT_COURSE_DURATION = "Duration";

    // COLUMNS VENDORS
    protected const string COL_VENDOR_ID = "VendorId";
    
    // COLUMNS SPECS
    protected const string COL_SPEC_ID = "SpecId";
    protected const string COL_SPEC_VALUE_ID = "SpecValueId";
    protected const string COL_SPEC_VALUE = "SpecValue";

    // PARAM CATEGORY
    protected const string PARAM_CATEGORY_ID = "@CategoryId";
    protected const string PARAM_CATEGORY_PARENT_ID = "@ParentCategoryId";
    protected const string PARAM_CATEGORY_TIER = "@CategoryTier";
    protected const string PARAM_CATEGORY_NAME = "@Name";
    protected const string PARAM_CATEGORY_API_URL = "@ApiUrl";
    protected const string PARAM_CATEGORY_IMAGE_URL = "@ImageUrl";
    protected const string TVP_PRIMARY_CATEGORIES = "TVP_PrimaryCategoryIds";
    protected const string PARAM_PRIMARY_CATEGORIES = "@PrimaryCategories";
    protected const string PARAM_IS_GLOBAL = "@IsGlobal";
    
    // PARAM SPECS
    protected const string PARAM_SPEC_ID = "@SpecId";
    protected const string PARAM_SPEC_NAME = "@SpecName";
    protected const string PARAM_SPEC_VALUES = "@SpecValues";
    protected const string TVP_SPEC_VALUES = "TVP_SpecLookupValues";
    protected const string TVP_COL_SPEC_ID = "SpecValueId";
    protected const string TVP_COL_SPEC_VALUE = "SpecValue";

    // PARAM FEATURES
    protected const string PARAM_FEATURE_ID = "@FeatureId";
    protected const string PARAM_FEATURE_NAME = "@Name";
    protected const string PARAM_FEATURE_URL = "@Url";
    protected const string PARAM_FEATURE_IMAGE = "@Image";
    
    // PARAM USER
    protected const string PARAM_USER_ID = "@UserId";
    protected const string PARAM_USER_NAME = "@Username";
    protected const string PARAM_USER_EMAIL = "@Email";
    protected const string PARAM_USER_NAME_OR_EMAIL = "@NameOrEmail";
    protected const string PARAM_USER_PHONE = "@Phone";
    protected const string PARAM_USER_PASSWORD_HASH = "@PasswordHash";
    protected const string PARAM_USER_PASSWORD_SALT = "@PasswordSalt";
    
    // PARAM SESSION
    protected const string PARAM_SESSION_ID = "@SessionId";
    protected const string PARAM_SESSION_ACTIVE = "@SessionActive";
    protected const string PARAM_SESSION_IP_ADDRESS = "@IpAddress";
    protected const string PARAM_SESSION_DEVICE_FINGERPRINT = "@Fingerprint";
    protected const string PARAM_SESSION_HASH = "@SessionHash";
    protected const string PARAM_SESSION_SALT = "@SessionSalt";
    
    // PARAM PRODUCT
    protected const string PARAM_PRODUCT_ID = "@ProductId";
    
    // PARAM FEATURES
    protected const string PARAM_FEATURE_IMAGE_URL = "@FeatureImageUrl";

    // PARAM CART
    protected const string PARAM_CART_ITEMS = "@CartItems";
    protected const string PARAM_CART_QUANTITY = "@ItemQuantity";
    protected const string TVP_CART_ITEMS = "TVP_CartItems";
    protected const string TVP_COL_CART_PRODUCT_ID = "ProductId";
    protected const string TVP_COL_CART_ITEM_QUANTITY = "ItemQuantity";

    // PARAM VARIANTS
    protected const string PARAM_VARIANT_ID = "@VariantId";
    protected const string PARAM_VARIANT_NAME = "@VariantName";
    protected const string PARAM_VARIANT_VALUES = "@VariantValues";
    protected const string TVP_VARIANT_VALUES = "TVP_VariantValues";
    protected const string TVP_COL_VARIANT_VALUE_ID = "VariantValueId";
    protected const string TVP_COL_VARIANT_VALUE = "VariantValue";
    
    // PARAM VENDORS
    protected const string PARAM_VENDOR_ID = "@VendorId";
    protected const string PARAM_VENDOR_NAME = "@VendorName";
    protected const string PARAM_VENDOR_URL = "@VendorUrl";

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
        while ( true )
        {
            try
            {
                await using SqlConnection connection = await _dbContext.GetOpenConnection();
                return await dapperQueryDelegate.Invoke( connection, dynamicSql, dynamicParams );
            }
            catch ( TimeoutException timeout )
            {
                currentRetry++;

                if ( currentRetry >= MAX_RETRIES )
                    throw new RepositoryException( timeout.Message, timeout );

                await Task.Delay( RETRY_DELAY_MILLISECONDS );
            }
            catch ( SqlException sqlException )
            {
                throw new RepositoryException( sqlException.Message, sqlException );
            }
            catch ( Exception e )
            {
                throw new RepositoryException( e.Message, e );
            }
        }
    }
    protected async Task<T?> TryQueryTransactionAsync<T>( DapperQueryTransactionDelegate<T> dapperQueryDelegate, DynamicParameters? dynamicParams = null, string? dynamicSql = null )
    {
        SqlConnection? connection = null;
        DbTransaction? transaction = null;

        int currentRetry = 0;
        while ( true )
        {
            try
            {
                connection = await _dbContext.GetOpenConnection();
                transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted );

                T? transactionResult = await dapperQueryDelegate.Invoke( connection, transaction, dynamicSql, dynamicParams );
                await transaction.CommitAsync();
                return transactionResult;
            }
            catch ( TimeoutException timeout )
            {
                currentRetry++;

                if ( currentRetry >= MAX_RETRIES )
                    throw new RepositoryException( timeout.Message, timeout );

                await Task.Delay( RETRY_DELAY_MILLISECONDS );
            }
            catch ( SqlException sqlException )
            {
                throw new RepositoryException( sqlException.Message, sqlException );
            }
            catch ( Exception e )
            {
                throw new RepositoryException( e.Message, e );
            }
            finally
            {
                if ( transaction?.Connection is not null )
                    await transaction.RollbackAsync();
                
                if ( transaction is not null )
                    await transaction.DisposeAsync();
                
                if ( connection is not null )
                {
                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                }
            }    
        }
    }

    protected static async Task<IEnumerable<T>?> Query<T>( SqlConnection connection, string? sql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<T>( sql, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    protected static async Task<IEnumerable<T>?> QueryTransaction<T>( SqlConnection connection, DbTransaction transaction, string? sql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<T>( sql, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
    }
    protected static async Task<T> QuerySingleOrDefault<T>( SqlConnection connection, string? sql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleOrDefaultAsync<T>( sql, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    protected static async Task<T> QuerySingleOrDefaultTransaction<T>( SqlConnection connection, DbTransaction transaction, string? sql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleOrDefaultAsync<T>( sql, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
    }
    protected static async Task<bool> Execute( SqlConnection connection, DbTransaction transaction, string? sql, DynamicParameters? dynamicParams )
    {
        int rows = await connection.ExecuteAsync( sql, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
        return rows > 0;
    }
    
    protected static DataTable GetPrimaryCategoriesTable( List<int> categories )
    {
        var table = new DataTable();
        table.Columns.Add( COL_CATEGORY_ID, typeof( int ) );
        
        foreach ( int id in categories )
            table.Rows.Add( id );
        
        return table;
    }
    protected static DataTable GetStringValuesTable( string valuesString, string idCol, string valueCol )
    {
        List<string> values = valuesString
            .Split( ',' )
            .Select( s => s.Trim() ) // Trims whitespace from each item.
            .ToList();

        var table = new DataTable();

        table.Columns.Add( idCol, typeof( int ) );
        table.Columns.Add( valueCol, typeof( string ) );

        for ( int i = 0; i < values.Count; i++ )
        {
            DataRow row = table.NewRow();
            row[ idCol ] = i + 1;
            row[ valueCol ] = values[ i ];
            table.Rows.Add( row );
        }

        return table;
    }
}