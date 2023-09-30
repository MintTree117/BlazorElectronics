namespace BlazorElectronics.Shared;

public sealed class DtoResponse<T>
{
    public DtoResponse( string? message )
    {
        Success = false;
        Message = message ?? DtoUtility.GetDefaultMessage( DtoDefaultMessage.FAILURE );
    }
    public DtoResponse( DtoResponse<T> response )
    {
        Data = response.Data;
        
        Success = response.Success;
        if ( Data == null )
            Success = false;
        
        Message = response.Message;
        if ( string.IsNullOrEmpty( Message ) )
            Message = Success ? DtoUtility.GetDefaultMessage( DtoDefaultMessage.SUCCESS ) : DtoUtility.GetDefaultMessage( DtoDefaultMessage.FAILURE );
    }
    public DtoResponse( T? data, bool success, string message )
    {
        Data = data;
        Success = success;
        Message = message;
    }
    public DtoResponse( T? data, bool success, DtoDefaultMessage defaultMessage )
    {
        Data = data;
        Success = success;
        Message = DtoUtility.GetDefaultMessage( defaultMessage );
    }
    public DtoResponse( T? data, bool success, string? recievedMessage, DtoDefaultMessage defaultMessage )
    {
        Data = data;
        Success = success;
        Message = recievedMessage ?? DtoUtility.GetDefaultMessage( defaultMessage );
    }

    public bool IsSuccessful()
    {
        return Data != null && Success;
    }
    
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}