using Blazored.LocalStorage;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Orders;

namespace BlazorElectronics.Client.Services.Orders;

public sealed class OrderServiceClient : UserServiceClient, IOrderServiceClient
{
    const string API_PATH = "api/orders";
    const string API_PATH_GET_ORDERS = API_PATH + "/get-summary";
    const string API_PATH_ORDER_DETAILS = API_PATH + "/get-details";
    
    public OrderServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<List<OrderOverviewDto>?>> GetOrders()
    {
        return await TryUserGetRequest<List<OrderOverviewDto>?>( API_PATH_GET_ORDERS );
    }
    public async Task<ServiceReply<OrderDetailsDto?>> GetOrderDetails( int orderId )
    {
        return await TryUserGetRequest<OrderDetailsDto?>( API_PATH_ORDER_DETAILS, GetOrderIdParam( orderId ) );
    }
    
    static Dictionary<string, object> GetUsernameParam( string username )
    {
        return new Dictionary<string, object> { { "username", username } };
    }
    static Dictionary<string, object> GetOrderIdParam( int orderId )
    {
        return new Dictionary<string, object> { { "orderId", orderId } };
    }
}