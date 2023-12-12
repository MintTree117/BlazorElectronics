namespace BlazorElectronics.Server.Core.Models.Sessions;

public sealed class SessionDto
{
    public SessionDto( int id, string token )
    {
        Id = id;
        Token = token;
    }
    
    public int Id { get; }
    public string Token { get; }
}