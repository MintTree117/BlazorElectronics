using Blazored.LocalStorage;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Checkout;

public sealed class CheckoutServiceClient : UserServiceClient, ICheckoutServiceClient
{
    const string API_PATH_PLACE_ORDER = "api/payment/checkout";
    
    public CheckoutServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public async Task<ServiceReply<string?>> PlaceOrder()
    {
        return await TryUserPostRequest<string?>( API_PATH_PLACE_ORDER );
    }
}