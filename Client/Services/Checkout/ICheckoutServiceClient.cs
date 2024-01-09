using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Checkout;

public interface ICheckoutServiceClient
{
    Task<ServiceReply<string?>> PlaceOrder();
}