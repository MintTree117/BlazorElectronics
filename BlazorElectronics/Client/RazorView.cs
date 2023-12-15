using System.Collections.Specialized;
using System.Web;
using BlazorElectronics.Client.Pages;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client;

public abstract class RazorView : ComponentBase
{
    [Inject] protected ILogger<PageView> Logger { get; init; } = default!;
    [Inject] protected NavigationManager NavManager { get; init; } = default!;

    public static Dictionary<string, string> ParseQuery( string queryString )
    {
        NameValueCollection query = HttpUtility.ParseQueryString( queryString );
        var queryDictionary = new Dictionary<string, string>();
        foreach ( string? key in query.AllKeys )
        {
            if ( string.IsNullOrWhiteSpace( key ) )
                continue;

            if ( string.IsNullOrWhiteSpace( query[ key ] ) )
                continue;
            
            queryDictionary.Add( key, query[ key ]! );
        }
        return queryDictionary;
    }
}