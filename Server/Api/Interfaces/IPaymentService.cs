using Stripe.Checkout;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IPaymentService
{
    Task<ServiceReply<Session>> CreateCheckoutSession( int userId );
    Task<ServiceReply<bool>> FulfillOrder( HttpRequest request );
}