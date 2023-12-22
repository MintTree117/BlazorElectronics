using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Web;
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

    protected async Task<ServiceReply<T?>> TryGetRequest<T>( string apiPath, Dictionary<string, object>? parameters = null )
    {
        try
        {
            string path = GetQueryParameters( apiPath, parameters );
            HttpResponseMessage httpResponse = await Http.GetAsync( path );
            return await HandleHttpResponse<T?>( httpResponse, "Get" );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T?>( e, "Get" );
        }
    }
    protected async Task<ServiceReply<T?>> TryPostRequest<T>( string apiPath, object? body = null )
    {
        try
        {
            HttpResponseMessage httpResponse = await Http.PostAsJsonAsync( apiPath, body );
            return await HandleHttpResponse<T?>( httpResponse, "Post" );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T?>( e, "Post" );
        }
    }
    protected async Task<ServiceReply<T?>> TryPutRequest<T>( string apiPath, object? body = null )
    {
        try
        {
            HttpResponseMessage httpResponse = await Http.PutAsJsonAsync( apiPath, body );
            return await HandleHttpResponse<T?>( httpResponse, "Put" );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T?>( e, "Put" );
        }
    }
    protected async Task<ServiceReply<T?>> TryDeleteRequest<T>( string apiPath, Dictionary<string, object>? parameters = null )
    {
        try
        {
            string path = GetQueryParameters( apiPath, parameters );
            HttpResponseMessage httpResponse = await Http.DeleteAsync( path );
            return await HandleHttpResponse<T?>( httpResponse, "Delete" );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T?>( e, "Delete" );
        }
    }

    protected static Dictionary<string, object> GetIdParam( int id )
    {
        return new Dictionary<string, object>
        {
            { "Id", id }
        };
    }

    static string GetQueryParameters( string apiPath, Dictionary<string, object>? parameters )
    {
        if ( parameters is null )
            return apiPath;
        
        NameValueCollection query = HttpUtility.ParseQueryString( string.Empty );
        
        foreach ( KeyValuePair<string, object> param in parameters )
        {
            query[ param.Key ] = param.Value.ToString();
        }

        return $"{apiPath}?{query}";
    }
    async Task<ServiceReply<T?>> HandleHttpResponse<T>( HttpResponseMessage httpResponse, string requestTypeName )
    {
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
                Logger.LogError( $"{requestTypeName}: Bad request: {errorContent}" );
                return new ServiceReply<T?>( ServiceErrorType.ValidationError, errorContent );

            case System.Net.HttpStatusCode.NotFound:
                Logger.LogError( $"{requestTypeName}: Not found: {errorContent}" );
                return new ServiceReply<T?>( ServiceErrorType.NotFound, errorContent );

            case System.Net.HttpStatusCode.Unauthorized:
                Logger.LogError( $"{requestTypeName}: Unauthorized: {errorContent}" );
                return new ServiceReply<T?>( ServiceErrorType.Unauthorized, errorContent );

            case System.Net.HttpStatusCode.Conflict:
                Logger.LogError( $"{requestTypeName}: Conflict: {errorContent}" );
                return new ServiceReply<T?>( ServiceErrorType.Conflict, errorContent );

            case System.Net.HttpStatusCode.InternalServerError:
                Logger.LogError( $"{requestTypeName}: Server error: {errorContent}" );
                return new ServiceReply<T?>( ServiceErrorType.ServerError, errorContent );

            default:
                Logger.LogError( $"{requestTypeName}: Other error: {httpResponse.StatusCode}, Content: {errorContent}" );
                return new ServiceReply<T?>( ServiceErrorType.ServerError, $"Error: {httpResponse.StatusCode}" );
        }
    }
    ServiceReply<T?> HandleHttpException<T>( Exception e, string requestType )
    {
        Logger.LogError( e, $"{requestType}: Exception occurred while sending API request." );
        return new ServiceReply<T?>( ServiceErrorType.ServerError, e.Message );
    }
}