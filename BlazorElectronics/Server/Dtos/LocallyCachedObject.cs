namespace BlazorElectronics.Server.Dtos;

public abstract class LocallyCachedObject
{
    readonly DateTime LastFetched;

    protected LocallyCachedObject()
    {
        LastFetched = DateTime.Now;
    }

    public bool IsValid( int maxHours )
    {
        return ( DateTime.Now - LastFetched ).TotalHours < maxHours;
    }
}