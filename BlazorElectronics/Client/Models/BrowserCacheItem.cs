namespace BlazorElectronics.Client.Models;

public sealed class BrowserCacheItem<T>
{
    public BrowserCacheItem()
    {
        
    }
    public BrowserCacheItem( T data )
    {
        Data = data;
    }

    public bool IsValid( int lifetime )
    {
        return ( DateTime.Now.Ticks - _dateCreated.Ticks ) < lifetime;
    }

    public T Data { get; private set; } = default!;
    DateTime _dateCreated = DateTime.Now;
}