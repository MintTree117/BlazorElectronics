namespace BlazorElectronics.Shared;

public enum DtoDefaultMessage
{
    NULL,
    FAILURE,
    SUCCESS
}

public record ApiReply<T>
{
    const string MESSAGE_RESPONSE_ERROR = "Failed to produce a proper response message!";
    const string MESSAGE_RESPONSE_NULL = "Service response is null!";
    const string MESSAGE_RESPONSE_FAILURE = "Service returned failure without a message!";
    const string MESSAGE_RESPONSE_SUCCESS = "Service returned success without a message!";
    
    public ApiReply()
    {
        
    }
    public ApiReply( ServiceErrorType errorType, string? message = null )
    {
        Data = default;
        Success = false;
        ErrorType = errorType;
        Message = message ?? GetDefaultMessage( DtoDefaultMessage.FAILURE );
    }
    public ApiReply( T data )
    {
        Data = data;
        Success = true;
        Message = string.Empty;
    }

    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
    public ServiceErrorType ErrorType { get; init; } = ServiceErrorType.None;

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