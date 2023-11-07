namespace BlazorElectronics.Server.Services;

public abstract class ApiService
{
    protected const string NO_DATA_FOUND_MESSAGE = "No data was found in cache or database!";
    protected const string INTERNAL_SERVER_ERROR_MESSAGE = "An internal server error occured!";
    
    protected ILogger _logger;
    
    public ApiService( ILogger logger )
    {
        _logger = logger;
    }
    
    protected static async Task<Reply<T>> ExecuteIoCall<T>( Func<Task<T>> func )
    {
        try
        {
            T result = await func();
            return new Reply<T>( result, true, "Operation successful" );
        }
        catch ( ServiceException ex )
        {
            return new Reply<T>( $"Operation failed: {ex.Message}" );
        }
    }
}