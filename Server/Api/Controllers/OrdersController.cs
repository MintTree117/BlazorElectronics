using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Orders;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class OrdersController : UserController
{
    readonly IOrderService _orderService;
    
    public OrdersController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, IOrderService orderService )
        : base( logger, userAccountService, sessionService )
    {
        _orderService = orderService;
    }

    [HttpGet( "orders" )]
    public async Task<ActionResult<bool>> GetOrders()
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<List<OrderOverviewDto>?> orderReply = await _orderService.GetOrders( userReply.Data );
        return GetReturnFromReply( orderReply );
    }
    [HttpGet( "order-details" )]
    public async Task<ActionResult<bool>> GetOrderDetails( int orderId )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<OrderDetailsDto?> orderReply = await _orderService.GetOrderDetails( orderId );
        return GetReturnFromReply( orderReply );
    }
}