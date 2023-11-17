namespace BlazorElectronics.Server.Services;

public abstract class ApiService<T>
{
    protected const string NO_DATA_FOUND_MESSAGE = "No data was found in cache or database!";
    protected const string INTERNAL_SERVER_ERROR_MESSAGE = "An internal server error occured!";
    
    protected ILogger<T> _logger;
    
    public ApiService( ILogger<T> logger )
    {
        _logger = logger;
    }
    
    protected static async Task<ApiReply<T>> ExecuteIoCall<T>( Func<Task<T>> func )
    {
        try
        {
            T result = await func();
            return new ApiReply<T>( result, true, "Operation successful" );
        }
        catch ( ServiceException ex )
        {
            return new ApiReply<T>( $"Operation failed: {ex.Message}" );
        }
    }
}