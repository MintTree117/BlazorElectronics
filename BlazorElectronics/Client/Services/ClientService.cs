using System.Net.Http.Json;
using Blazored.LocalStorage;
using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services;

public abstract class ClientService
{
    protected readonly ILogger<ClientService> Logger;
    protected readonly HttpClient Http;
    protected readonly ILocalStorageService Storage;
    
    protected ClientService( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
    {
        Logger = logger;
        Http = http;
        Storage = storage;
    }

    protected async Task<ApiReply<T>> TryPostRequest<T>( string apiPath, object? data = null )
    {
        try
        {
            HttpResponseMessage httpResponse = await Http.PostAsJsonAsync( apiPath, data );

            if ( httpResponse.IsSuccessStatusCode )
            {
                var apiReply = await httpResponse.Content.ReadFromJsonAsync<ApiReply<T>>();
                return apiReply ?? new ApiReply<T>( default, false, "No content returned from API." );
            }
            else if ( httpResponse.StatusCode is System.Net.HttpStatusCode.BadRequest )
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                Logger.LogError( $"Bad request: {errorContent}" );
                return new ApiReply<T>( default, false, errorContent );
            }
            else
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                Logger.LogError( $"Error: {httpResponse.StatusCode}, Content: {errorContent}" );
                return new ApiReply<T>( default, false, $"Error: {httpResponse.StatusCode}" );
            }
        }
        catch ( Exception e )
        {
            Logger.LogError( e, "Exception occurred while sending API request." );
            return new ApiReply<T>( default, false, e.Message );
        }
    }
}