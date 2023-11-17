namespace BlazorElectronics.Shared;

public enum DtoDefaultMessage
{
    NULL,
    FAILURE,
    SUCCESS
}

public sealed class ApiReply<T>
{
    const string MESSAGE_RESPONSE_ERROR = "Failed to produce a proper response message!";
    const string MESSAGE_RESPONSE_NULL = "Service response is null!";
    const string MESSAGE_RESPONSE_FAILURE = "Service returned failure without a message!";
    const string MESSAGE_RESPONSE_SUCCESS = "Service returned success without a message!";
    
    public ApiReply()
    {
        
    }
    public ApiReply( string? message )
    {
        Data = default;
        Success = false;
        Message = message ?? GetDefaultMessage( DtoDefaultMessage.FAILURE );
    }
    public ApiReply( ApiReply<T> response )
    {
        Data = response.Data;
        
        Success = response.Success;
        if ( Data is null )
            Success = false;
        
        Message = response.Message;
        if ( string.IsNullOrEmpty( Message ) )
            Message = Success ? GetDefaultMessage( DtoDefaultMessage.SUCCESS ) : GetDefaultMessage( DtoDefaultMessage.FAILURE );
    }
    public ApiReply( T data )
    {
        Data = data;
        Success = true;
        Message = string.Empty;
    }
    public ApiReply( T? data, bool success, string message )
    {
        Data = data;
        Success = success;
        Message = message;
    }
    public ApiReply( T? data, bool success, DtoDefaultMessage defaultMessage )
    {
        Data = data;
        Success = success;
        Message = GetDefaultMessage( defaultMessage );
    }
    public ApiReply( T? data, bool success, string? recievedMessage, DtoDefaultMessage defaultMessage )
    {
        Data = data;
        Success = success;
        Message = recievedMessage ?? GetDefaultMessage( defaultMessage );
    }

    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    static string GetDefaultMessage( DtoDefaultMessage defaultMessage )
    {
        return defaultMessage switch {
            DtoDefaultMessage.NULL => MESSAGE_RESPONSE_NULL,
            DtoDefaultMessage.FAILURE => MESSAGE_RESPONSE_FAILURE,
            DtoDefaultMessage.SUCCESS => MESSAGE_RESPONSE_SUCCESS,
            _ => MESSAGE_RESPONSE_ERROR
        };
    }
}