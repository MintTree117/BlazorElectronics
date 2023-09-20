namespace BlazorElectronics.Shared;

public sealed class ControllerResponse<T>
{
    public ControllerResponse( T data, bool success, string message )
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}