namespace BlazorElectronics.Server.Dtos.Users;

public sealed class ValidatedDataRequest<T>
{
    public ValidatedDataRequest()
    {
        
    }

    public ValidatedDataRequest( int userId, T? dto = default )
    {
        UserId = userId;
        Dto = dto;
    }

    public int UserId { get; set; }
    public T? Dto { get; set; }
}