namespace BlazorElectronics.Server.Dtos;

public class CachedObject<T>
{
    public readonly T Object;
    readonly DateTime LastFetched;

    public CachedObject( T o )
    {
        Object = o;
        LastFetched = DateTime.Now;
    }

    public bool IsValid( int maxHours )
    {
        return ( DateTime.Now - LastFetched ).TotalHours < maxHours;
    }
}