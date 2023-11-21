namespace BlazorElectronics.Server.Dtos.Users;

public sealed class ValidatedUserApiRequest<T>
{
    public ValidatedUserApiRequest()
    {
        
    }

    public ValidatedUserApiRequest( int userId, T? dto = default )
    {
        UserId = userId;
        Dto = dto;
    }

    public int UserId { get; set; }
    public T? Dto { get; set; }
}