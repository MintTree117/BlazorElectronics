using System.Net.Http.Json;
using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Enums;

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
    
    protected async Task<ServiceReply<T?>> TryGetRequest<T>( string apiPath )
    {
        try
        {
            HttpResponseMessage httpResponse = await Http.GetAsync( apiPath );

            if ( httpResponse.IsSuccessStatusCode )
            {
                var getReply = await httpResponse.Content.ReadFromJsonAsync<T>();

                return getReply is not null
                    ? new ServiceReply<T?>( getReply )
                    : new ServiceReply<T?>( ServiceErrorType.NotFound, "No data returned from request" );
            }

            string errorContent = await httpResponse.Content.ReadAsStringAsync();

            switch ( httpResponse.StatusCode )
            {
                case System.Net.HttpStatusCode.BadRequest:
                    Logger.LogError( $"TryGetRequest: Bad request: {errorContent}" );
                    return new ServiceReply<T?>( ServiceErrorType.ValidationError, errorContent );

                case System.Net.HttpStatusCode.NotFound:
                    Logger.LogError( $"TryGetRequest: Not found: {errorContent}" );
                    return new ServiceReply<T?>( ServiceErrorType.NotFound, errorContent );

                case System.Net.HttpStatusCode.Unauthorized:
                    Logger.LogError( $"TryGetRequest: Unauthorized: {errorContent}" );
                    return new ServiceReply<T?>( ServiceErrorType.Unauthorized, errorContent );

                case System.Net.HttpStatusCode.Conflict:
                    Logger.LogError( $"TryGetRequest: Conflict: {errorContent}" );
                    return new ServiceReply<T?>( ServiceErrorType.Conflict, errorContent );

                case System.Net.HttpStatusCode.InternalServerError:
                    Logger.LogError( $"TryGetRequest: Server error: {errorContent}" );
                    return new ServiceReply<T?>( ServiceErrorType.ServerError, errorContent );

                default:
                    Logger.LogError( $"TryGetRequest: Other error: {httpResponse.StatusCode}, Content: {errorContent}" );
                    return new ServiceReply<T?>( ServiceErrorType.ServerError, $"Error: {httpResponse.StatusCode}" );
            }
        }
        catch ( Exception e )
        {
            Logger.LogError( e, "TryGetRequest: Exception occurred while sending API request." );
            return new ServiceReply<T?>( ServiceErrorType.ServerError, e.Message );
        }
    }
    protected async Task<ServiceReply<T?>> TryPostRequest<T>( string apiPath, object? data = null )
    {
        try
        {
            HttpResponseMessage httpResponse = await Http.PostAsJsonAsync( apiPath, data );

            if ( httpResponse.IsSuccessStatusCode )
            {
                var postReply = await httpResponse.Content.ReadFromJsonAsync<T>();

                return postReply is not null
                    ? new ServiceReply<T?>( postReply )
                    : new ServiceReply<T?>( ServiceErrorType.NotFound, "No data returned from request" );
            }
            
            string errorContent = await httpResponse.Content.ReadAsStringAsync();

            switch ( httpResponse.StatusCode )
            {
                case System.Net.HttpStatusCode.BadRequest:
                    Logger.LogError( $"TryPostRequest: Bad request: {errorContent}" );
                    return new ServiceReply<T?>( ServiceErrorType.ValidationError, errorContent );

                case System.Net.HttpStatusCode.NotFound:
                    Logger.LogError( $"TryPostRequest: Not found: {errorContent}" );
                    return new ServiceReply<T?>( ServiceErrorType.NotFound, errorContent );

                case System.Net.HttpStatusCode.Unauthorized:
                    Logger.LogError( $"TryPostRequest: Unauthorized: {errorContent}" );
                    return new ServiceReply<T?>( ServiceErrorType.Unauthorized, errorContent );

                case System.Net.HttpStatusCode.Conflict:
                    Logger.LogError( $"TryPostRequest: Conflict: {errorContent}" );
                    return new ServiceReply<T?>( ServiceErrorType.Conflict, errorContent );

                case System.Net.HttpStatusCode.InternalServerError:
                    Logger.LogError( $"TryPostRequest: Server error: {errorContent}" );
                    return new ServiceReply<T?>( ServiceErrorType.ServerError, errorContent );

                default:
                    Logger.LogError( $"TryPostRequest: Other error: {httpResponse.StatusCode}, Content: {errorContent}" );
                    return new ServiceReply<T?>( ServiceErrorType.ServerError, $"Error: {httpResponse.StatusCode}" );
            }
        }
        catch ( Exception e )
        {
            Logger.LogError( e, "TryPostRequest: Exception occurred while sending API request." );
            return new ServiceReply<T?>( ServiceErrorType.ServerError, e.Message );
        }
    }
}