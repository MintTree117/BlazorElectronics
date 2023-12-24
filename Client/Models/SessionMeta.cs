namespace BlazorElectronics.Client.Models;

public record SessionMeta
{
    public SessionMeta( SessionType type, string? username )
    {
        Type = type;
        Username = username;
    }
    
    public SessionType Type { get; init; }
    public string? Username { get; init; } = string.Empty;
}