using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared;

public enum DtoDefaultMessage
{
    NULL,
    FAILURE,
    SUCCESS
}

public record ServiceReply<T>
{
    const string MESSAGE_RESPONSE_ERROR = "Failed to produce a proper response message!";

    public ServiceReply()
    {
        
    }
    public ServiceReply( ServiceErrorType errorType, string? message = null )
    {
        Data = default;
        Success = false;
        ErrorType = errorType;
        Message = message ?? GetDefaultMessage( errorType );
    }
    public ServiceReply( T data )
    {
        Data = data;
        Success = true;
        Message = string.Empty;
    }

    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
    public ServiceErrorType ErrorType { get; init; } = ServiceErrorType.None;

    static string GetDefaultMessage( ServiceErrorType errorType )
    {
        return errorType switch {
            ServiceErrorType.IoError => "An IO error occured!",
            ServiceErrorType.Unauthorized => "Unauthorized!",
            ServiceErrorType.NotFound => "Data not found!",
            ServiceErrorType.ServerError => "Internal Server Error!",
            ServiceErrorType.ValidationError => "Validation failed!",
            _ => MESSAGE_RESPONSE_ERROR
        };
    }
}