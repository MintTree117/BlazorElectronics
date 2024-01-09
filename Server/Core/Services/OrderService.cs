using System.Text;
using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Orders;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Orders;
using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Server.Core.Services;

public sealed class OrderService : _ApiService, IOrderService
{
    readonly IOrderRepository _orderRepository;
    readonly ICartRepository _cartRepository;
    readonly IEmailService _emailService;
    
    public OrderService( ILogger<OrderService> logger, IOrderRepository orderRepository, ICartRepository cartRepository, IEmailService emailService )
        : base( logger )
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _emailService = emailService;
    }
    
    public async Task<ServiceReply<bool>> FulfillOrder( int userId, string email )
    {
        ServiceReply<CartDto?> cartReply = await TryGetCart( userId );

        if ( !cartReply.Success || cartReply.Payload is null )
            return new ServiceReply<bool>( cartReply.ErrorType, cartReply.Message );

        decimal totalPrice = 0;
        decimal totalDiscount = 0;
        List<OrderItemModel> orderItems = new();
        List<PromoCodeDto> promoCodes = new();

        foreach ( CartProductDto m in cartReply.Payload.Products )
        {
            decimal price = ( m.SalePrice ?? m.Price ) * m.Quantity;
            totalPrice += ( m.SalePrice ?? m.Price ) * m.Quantity;

            orderItems.Add( new OrderItemModel
            {
                ProductId = m.ProductId,
                Quantity = m.Quantity,
                TotalPrice = price
            } );
        }

        foreach ( PromoCodeDto p in cartReply.Payload.PromoCodes )
        {
            promoCodes.Add( p );
            totalDiscount += p.Discount;
        }

        totalPrice -= totalPrice * totalDiscount;

        OrderModel orderModel = new()
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            TotalPrice = totalPrice,
            OrderItems = orderItems,
            PromoCodes = promoCodes
        };

        ServiceReply<bool> orderReply = await TryPlaceOrder( orderModel );

        if ( !orderReply.Success )
            return new ServiceReply<bool>( orderReply.ErrorType, orderReply.Message );

        await TryDeleteCart( userId );

        SendOrderEmail( email, orderModel, cartReply.Payload );
        return new ServiceReply<bool>( true );
    }
    public async Task<ServiceReply<List<OrderOverviewDto>?>> GetOrders( int userId )
    {
        try
        {
            IEnumerable<OrderOverviewDto>? model = await _orderRepository.GetOrders( userId );

            return model is not null
                ? new ServiceReply<List<OrderOverviewDto>?>( model.ToList() )
                : new ServiceReply<List<OrderOverviewDto>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<OrderOverviewDto>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<OrderDetailsDto?>> GetOrderDetails( int orderId )
    {
        try
        {
            OrderDetailsModel? model = await _orderRepository.GetOrderDetails( orderId );
            OrderDetailsDto? dto = MapOrderDetails( model );

            return dto is not null
                ? new ServiceReply<OrderDetailsDto?>( dto )
                : new ServiceReply<OrderDetailsDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<OrderDetailsDto?>( ServiceErrorType.ServerError );
        }
    }

    void SendOrderEmail( string toEmail, OrderModel order, CartDto cart )
    {
        StringBuilder builder = new();

        builder.Append( "Thank you for completing your order! We hope you enjoy it." );
        builder.Append( "Order Summary:" );
        builder.Append( $"Order Date: {order.OrderDate}" );
        builder.Append( $"Order Total: {order.TotalPrice}" );
        builder.Append( "Items: " );
        foreach ( CartProductDto item in cart.Products )
            builder.Append( $"{item.Title}: x {item.Quantity}" );
        builder.Append( "Promotions: " );
        foreach ( PromoCodeDto promo in cart.PromoCodes )
            builder.Append( $"{promo.PromoCode}" );
        builder.Append( "You can review your order from your account: " );
        builder.Append( "https://blazormedia.azurewebsites.net/login" );

        _emailService.SendEmailAsync( toEmail, "BlazorMedia Order", builder.ToString() );
    }
    static OrderDetailsDto? MapOrderDetails( OrderDetailsModel? model )
    {
        if ( model?.Items is null || model.Meta is null )
            return null;

        return new OrderDetailsDto
        {
            OrderDate = model.Meta.OrderDate,
            TotalPrice = model.Meta.TotalPrice,
            Products = model.Items.ToList()
        };
    }

    async Task<ServiceReply<CartDto?>> TryGetCart( int userId )
    {
        try
        {
            CartDto? cart = await _cartRepository.Get( userId );

            return cart?.Products is null || cart.PromoCodes is null
                ? new ServiceReply<CartDto?>( ServiceErrorType.NotFound, "Failed to find cart for user!" )
                : new ServiceReply<CartDto?>( cart );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CartDto?>( ServiceErrorType.ServerError );
        }
    }
    async Task<ServiceReply<bool>> TryPlaceOrder( OrderModel orderModel )
    {
        try
        {
            bool placedOrder = await _orderRepository.InsertOrder( orderModel );

            return placedOrder
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.Conflict, "Failed to place order! Please contact support." );

        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    async Task TryDeleteCart( int userId )
    {
        try
        {
            await _cartRepository.DeleteCart( userId );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
        }
    }
}