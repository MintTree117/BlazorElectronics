namespace BlazorElectronics.Server.Services;

public abstract class Service
{
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