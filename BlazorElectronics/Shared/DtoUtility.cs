namespace BlazorElectronics.Shared;

public enum DtoDefaultMessage
{
    NULL,
    FAILURE,
    SUCCESS
}

public static class DtoUtility
{
    const string MESSAGE_RESPONSE_ERROR = "Failed to produce a proper response message!";
    const string MESSAGE_RESPONSE_NULL = "Service response is null!";
    const string MESSAGE_RESPONSE_FAILURE = "Service returned failure without a message!";
    const string MESSAGE_RESPONSE_SUCCESS = "Service returned success without a message!";

    public static string GetDefaultMessage( DtoDefaultMessage defaultMessage )
    {
        return defaultMessage switch {
            DtoDefaultMessage.NULL => MESSAGE_RESPONSE_NULL,
            DtoDefaultMessage.FAILURE => MESSAGE_RESPONSE_FAILURE,
            DtoDefaultMessage.SUCCESS => MESSAGE_RESPONSE_SUCCESS,
            _ => MESSAGE_RESPONSE_ERROR
        };
    }
}