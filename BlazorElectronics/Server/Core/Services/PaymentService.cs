using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Promos;
using BlazorElectronics.Shared.Users;
using Stripe;
using Stripe.Checkout;

namespace BlazorElectronics.Server.Core.Services;

public sealed class PaymentService : ApiService, IPaymentService
{
    readonly IOrderService _orderService;
    readonly IOrderRepository _orderRepository;
    readonly ICartRepository _cartRepository;
    readonly IUserAccountRepository _userRepository;

    const string secret = "whsec_ca95fb7851a7a56be88f4b891867980fc468cc280e654eac5c1983cf7f44e935";

    public PaymentService( ILogger<ApiService> logger, ICartRepository cartRepository, IUserAccountRepository userRepository, IOrderService orderService, IOrderRepository orderRepository )
        : base( logger )
    {
        _cartRepository = cartRepository;
        _userRepository = userRepository;
        _orderService = orderService;
        _orderRepository = orderRepository;
        StripeConfiguration.ApiKey = "sk_test_51OQcjnFBq2e5mEpMCMlHKmZspWwb4r4aYMP0N5tBBIzSYHr8PNuJvgLcucV7o7hCUlX3aTx9c1QEBDfKZe1sxpHV00obxjnJYs";
    }

    public async Task<ServiceReply<Session>> CreateCheckoutSession( int userId )
    {
        CartDto? cart;
        AccountDetailsDto? user;

        try
        {
            cart = await _cartRepository.GetCart( userId );
            user = await _userRepository.GetAccountDetails( userId );

            if ( cart is null || user is null )
                return new ServiceReply<Session>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<Session>( ServiceErrorType.ServerError );
        }

        decimal totalDiscount = 0;
        var lineItems = new List<SessionLineItemOptions>();

        foreach ( PromoCodeDto p in cart.PromoCodes )
        {
            if ( !p.IsActive || cart.PromoCodes.All( pc => pc.PromoCode != p.PromoCode ) )
                return new ServiceReply<Session>( ServiceErrorType.Conflict, "One or more promo codes are not available!" );
            
            totalDiscount += p.Discount;
        }

        totalDiscount = Math.Min( 1, totalDiscount );

        foreach ( CartProductDto m in cart.Products )
        {
            CartProductDto? cp = cart.Products.FirstOrDefault( i => i.ProductId == m.ProductId );

            if ( cp is null )
                return new ServiceReply<Session>( ServiceErrorType.Conflict, "One or more items are not available!" );

            if ( m.Price != cp.Price || m.SalePrice != cp.SalePrice )
                return new ServiceReply<Session>( ServiceErrorType.Conflict, "One or more products have changed price!" );

            decimal price = m.SalePrice ?? m.Price;
            price -= price * totalDiscount;
            
            lineItems.Add( new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = price * 100 ,
                    Currency = "cad",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = m.Title
                        //Images = new List<string> { p.Thumbnail }
                    }
                },
                Quantity = m.Quantity
            } );
        }

        var options = new SessionCreateOptions
        {
            CustomerEmail = user.Email,
            ShippingAddressCollection =
                new SessionShippingAddressCollectionOptions
                {
                    AllowedCountries = new List<string> { "CA" }
                },
            PaymentMethodTypes = new List<string>
            {
                "card"
            },
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = "https://localhost:7114/order-success",
            CancelUrl = "https://localhost:7114/cart"
        };

        var service = new Stripe.Checkout.SessionService();
        Session session = await service.CreateAsync( options );
        return new ServiceReply<Session>( session );
    }
    public async Task<ServiceReply<bool>> FulfillOrder( HttpRequest request )
    {
        var json = await new StreamReader( request.Body ).ReadToEndAsync();
        
        try
        {
            Event? stripeEvent = EventUtility.ConstructEvent(
                json,
                request.Headers[ "Stripe-Signature" ],
                secret
            );

            if ( stripeEvent.Type != Events.CheckoutSessionCompleted ) 
                return new ServiceReply<bool>( false );
            
            if ( stripeEvent.Data.Object is not Session session )
                return new ServiceReply<bool>( ServiceErrorType.ServerError );

            int userId = await _userRepository.GetIdByEmail( session.CustomerEmail );

            if ( userId <= 0 )
                return new ServiceReply<bool>( ServiceErrorType.NotFound );

            await _orderService.PlaceOrder( userId );
            return new ServiceReply<bool>( true );
        }
        catch ( StripeException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
}