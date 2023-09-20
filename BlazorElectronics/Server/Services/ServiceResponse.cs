namespace BlazorElectronics.Server.Services;

public sealed class ServiceResponse<T>
{
    public ServiceResponse( T data, bool success, string message )
    {
        Data = data;
        Success = success;
        Message = message;
    }

    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}