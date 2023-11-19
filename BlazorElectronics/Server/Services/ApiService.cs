namespace BlazorElectronics.Server.Services;

public abstract class ApiService
{
    protected const string NO_DATA_FOUND_MESSAGE = "No data was found in cache or database!";
    protected const string INTERNAL_SERVER_ERROR_MESSAGE = "An internal server error occured!";
    
    protected readonly ILogger<ApiService> Logger;
    
    public ApiService( ILogger<ApiService> logger )
    {
        Logger = logger;
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