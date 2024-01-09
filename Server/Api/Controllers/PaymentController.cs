using BlazorElectronics.Server.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class PaymentController : _UserController
{
    readonly IPaymentService _paymentService;

    public PaymentController( ILogger<PaymentController> logger, IUserAccountService userAccountService, ISessionService sessionService, IPaymentService paymentService )
        : base( logger, userAccountService, sessionService )
    {
        _paymentService = paymentService;
    }
    
    [HttpPost( "checkout" )]
    public async Task<ActionResult<string>> CreateCheckoutSession()
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<Session> paymentSessionReply = await _paymentService.CreateCheckoutSession( userReply.Payload );
        return GetReturnFromReply( paymentSessionReply );
    }

    [HttpPost( "fulfill" )]
    public async Task<ActionResult<bool>> FulfillOrder()
    {
        ServiceReply<bool> reply = await _paymentService.FulfillOrder( Request );
        return GetReturnFromReply( reply );
    }
}