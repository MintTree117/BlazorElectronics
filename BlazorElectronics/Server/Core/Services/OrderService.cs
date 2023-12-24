using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Orders;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Orders;
using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Server.Core.Services;

public sealed class OrderService : ApiService, IOrderService
{
    readonly IOrderRepository _orderRepository;
    readonly ICartRepository _cartRepository;
    
    public OrderService( ILogger<ApiService> logger, IOrderRepository orderRepository, ICartRepository cartRepository )
        : base( logger )
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
    }
    
    public async Task<ServiceReply<bool>> PlaceOrder( int userId )
    {
        CartDto? cartReply;

        try
        {
            cartReply = await _cartRepository.GetCart( userId );

            if ( cartReply?.Products is null || cartReply.PromoCodes is null )
                return new ServiceReply<bool>( ServiceErrorType.NotFound, "Failed to find cart for user!" );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }

        decimal totalPrice = 0;
        decimal totalDiscount = 0;
        List<OrderItemModel> orderItems = new();
        List<PromoCodeDto> promoCodes = new();

        foreach ( CartProductDto m in cartReply.Products )
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

        foreach ( PromoCodeDto p in cartReply.PromoCodes )
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
        
        try
        {
            bool placedOrder = await _orderRepository.InsertOrder( orderModel );

            if ( !placedOrder )
                return new ServiceReply<bool>( ServiceErrorType.Conflict, "Failed to place order!" );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }

        try
        {
            await _cartRepository.DeleteCart( userId );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }

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
}