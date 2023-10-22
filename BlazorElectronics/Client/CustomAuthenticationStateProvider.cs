using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorElectronics.Client;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    readonly ILocalStorageService _localStorage;
    readonly HttpClient _http;
    
    public CustomAuthenticationStateProvider( ILocalStorageService localStorage, HttpClient http )
    {
        _localStorage = localStorage;
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string authToken = await _localStorage.GetItemAsStringAsync( "authToken" );
        
        var identiity = new ClaimsIdentity();
        _http.DefaultRequestHeaders.Authorization = null;

        if ( !string.IsNullOrEmpty( authToken ) )
        {
            try
            {
                identiity = new ClaimsIdentity( ParseClaimsFromJwt( authToken ), "jwt" );
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue( "Bearer", authToken.Replace( "\"", "" ) );
            }
            catch
            {
                await _localStorage.RemoveItemAsync( "authToken" );
                identiity = new ClaimsIdentity();
            }
        }

        var user = new ClaimsPrincipal( identiity );
        var state = new AuthenticationState( user );

        NotifyAuthenticationStateChanged( Task.FromResult( state ) );

        return state;
    }

    static byte[] ParseBase64WithoutPadding( string base64 )
    {
        switch ( base64.Length % 4 )
        {
            case 2: base64 += "=="; 
                break;
            case 3: base64 += "=";
                break;
        }

        return Convert.FromBase64String( base64 );
    }
    static IEnumerable<Claim> ParseClaimsFromJwt( string jwt )
    {
        var payload = jwt.Split( '.' )[ 1 ];
        var jsonBytes = ParseBase64WithoutPadding( payload );
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>( jsonBytes );
        IEnumerable<Claim> claims = keyValuePairs.Select( kvp => new Claim( kvp.Key, kvp.Value.ToString() ) );
        return claims;
    }
}