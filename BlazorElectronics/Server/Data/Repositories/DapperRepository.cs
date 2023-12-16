using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Data.Repositories;

public abstract class DapperRepository
{
    // TABLES PRODUCTS
    protected const string TABLE_PRODUCTS = "Products";
    protected const string TABLE_PRODUCT_CATEGORIES = "Product_Categories";
    protected const string TABLE_PRODUCT_DESCRIPTIONS = "Product_Descriptions";
    protected const string TABLE_PRODUCT_IMAGES = "Product_Images";
    protected const string TABLE_PRODUCT_REVIEWS = "Product_Reviews";
    protected const string TABLE_PRODUCT_SPECS = "Product_Specs_Lookup";
    protected const string TABLE_PRODUCT_XML = "Product_Xml";

    // SHARED
    protected const string PARAM_IS_GLOBAL = "@IsGlobal";
    protected const string PARAM_ROWS = "@Rows";
    protected const string PARAM_OFFSET = "@Offset";
    
    // CATEGORIES
    protected const string COL_CATEGORY_ID = "CategoryId";
    protected const string COL_CATEGORY_PARENT_ID = "ParentCategoryId";
    protected const string COL_CATEGORY_TIER = "Tier";
    protected const string COL_CATEGORY_NAME = "Name";
    protected const string COL_CATEGORY_URL = "ApiUrl";
    protected const string COL_CATEGORY_IMAGE = "ImageUrl";
    protected const string TVP_CATEGORY_IDS = "TVP_CategoryIds";
    protected const string TVP_CATEGORIES = "TVP_Categories";
    protected const string PARAM_CATEGORY_IDS = $"@{TVP_CATEGORY_IDS}";
    protected const string PARAM_CATEGORIES = $"@{TVP_CATEGORIES}";
    protected const string PARAM_CATEGORY_ID = $"@{COL_CATEGORY_ID}";
    protected const string PARAM_CATEGORY_PARENT_ID = $"@{COL_CATEGORY_PARENT_ID}";
    protected const string PARAM_CATEGORY_TIER = $"@{COL_CATEGORY_TIER}";
    protected const string PARAM_CATEGORY_NAME = $"@{COL_CATEGORY_NAME}";
    protected const string PARAM_CATEGORY_API_URL = $"@{COL_CATEGORY_URL}";
    protected const string PARAM_CATEGORY_IMAGE_URL = $"@{COL_CATEGORY_IMAGE}";

    // PRODUCTS
    protected const string COL_PRODUCT_ID = "ProductId";
    protected const string COL_PRODUCT_TITLE = "Title";
    protected const string COL_PRODUCT_REVIEW_ID = "ReviewId";
    protected const string COL_PRODUCT_RATING = "Rating";
    protected const string COL_PRODUCT_REVIEW = "Review";
    protected const string COL_PRODUCT_THUMBNAIL = "Thumbnail";
    protected const string COL_PRODUCT_RELEASE_DATE = "ReleaseDate";
    protected const string COL_PRODUCT_PRICE = "Price";
    protected const string COL_PRODUCT_SALE_PRICE = "SalePrice";
    protected const string COL_PRODUCT_IS_FEATURED = "IsFeatured";
    protected const string COL_PRODUCT_NUMBER_SOLD = "NumberSold";
    protected const string COL_PRODUCT_NUMBER_REVIEWS = "NumberReviews";
    protected const string COL_PRODUCT_DESCR = "Description";
    protected const string COL_PRODUCT_XML = "XmlSpecs";
    protected const string COL_PRODUCT_IMAGE = "ImageUrl";
    protected const string TVP_PRODUCT_IMAGES = "TVP_Images";
    protected const string PARAM_PRODUCT_ID = $"@{COL_PRODUCT_ID}";
    protected const string PARAM_PRODUCT_TITLE = $"@{COL_PRODUCT_TITLE}";
    protected const string PARAM_PRODUCT_THUMBNAIL = $"@{COL_PRODUCT_THUMBNAIL}";
    protected const string PARAM_PRODUCT_PRICE = $"@{COL_PRODUCT_PRICE}";
    protected const string PARAM_PRODUCT_SALE_PRICE = $"@{COL_PRODUCT_SALE_PRICE}";
    protected const string PARAM_PRODUCT_RELEASE_DATE = $"@{COL_PRODUCT_RELEASE_DATE}";
    protected const string PARAM_PRODUCT_DESCR = $"@{COL_PRODUCT_DESCR}";
    protected const string PARAM_PRODUCT_REVIEW_ID = $"@{COL_PRODUCT_REVIEW_ID}";
    protected const string PARAM_PRODUCT_RATING = $"@{COL_PRODUCT_RATING}";
    protected const string PARAM_PRODUCT_REVIEW = $"@{COL_PRODUCT_REVIEW}";
    protected const string PARAM_PRODUCT_XML = $"@{COL_PRODUCT_XML}";
    protected const string PARAM_PRODUCT_IMAGES = $"@{TVP_PRODUCT_IMAGES}";
    
    
    // VENDORS
    protected const string COL_VENDOR_ID = "VendorId";
    protected const string PARAM_VENDOR_ID = "@VendorId";
    protected const string PARAM_VENDOR_NAME = "@VendorName";
    protected const string PARAM_VENDOR_URL = "@VendorUrl";
    
    // SPECS
    protected const string COL_SPEC_ID = "SpecId";
    protected const string COL_SPEC_VALUE_ID = "SpecValueId";
    protected const string COL_SPEC_VALUE = "SpecValue";
    protected const string COL_SPEC_AVOID = "IsAvoid";
    protected const string TVP_SPECS = "TVP_Specs";
    protected const string TVP_SPEC_VALUES = "TVP_SpecValues";
    protected const string PARAM_SPECS = $"@{TVP_SPECS}";
    protected const string PARAM_SPEC_VALUES = $"@{TVP_SPEC_VALUES}";
    protected const string PARAM_SPEC_ID = $"@{COL_SPEC_ID}";
    protected const string PARAM_SPEC_NAME = "@SpecName";
    protected const string PARAM_SPEC_AVOID = $"@{COL_SPEC_AVOID}";

    // FEATURES
    protected const string PARAM_FEATURE_ID = "@FeatureId";
    protected const string PARAM_FEATURE_NAME = "@Name";
    protected const string PARAM_FEATURE_URL = "@Url";
    protected const string PARAM_FEATURE_IMAGE = "@Image";

    // USER
    protected const string COL_USER_ID = "UserId";
    protected const string PARAM_USER_ID = $"@{COL_USER_ID}";
    protected const string PARAM_USER_NAME = "@Username";
    protected const string PARAM_USER_EMAIL = "@Email";
    protected const string PARAM_USER_NAME_OR_EMAIL = "@NameOrEmail";
    protected const string PARAM_USER_PHONE = "@Phone";
    protected const string PARAM_USER_PASSWORD_HASH = "@PasswordHash";
    protected const string PARAM_USER_PASSWORD_SALT = "@PasswordSalt";

    // SESSION
    protected const string PARAM_SESSION_ID = "@SessionId";
    protected const string PARAM_SESSION_ACTIVE = "@SessionActive";
    protected const string PARAM_SESSION_IP_ADDRESS = "@IpAddress";
    protected const string PARAM_SESSION_DEVICE_FINGERPRINT = "@Fingerprint";
    protected const string PARAM_SESSION_HASH = "@SessionHash";
    protected const string PARAM_SESSION_SALT = "@SessionSalt";

    // CART
    protected const string COL_CART_ITEM_QUANTITY = "Quantity";
    protected const string TVP_CART_ITEMS = "TVP_CartItems";
    protected const string PARAM_CART_ITEMS = $"@{TVP_CART_ITEMS}";
    protected const string PARAM_CART_QUANTITY = $"@{COL_CART_ITEM_QUANTITY}";

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