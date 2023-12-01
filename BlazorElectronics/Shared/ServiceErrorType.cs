namespace BlazorElectronics.Shared;

public enum ServiceErrorType
{
    None,
    IoError,
    ValidationError,
    NotFound,
    Unauthorized,
    Conflict,
    ServerError
}