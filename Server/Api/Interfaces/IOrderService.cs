using BlazorElectronics.Shared.Orders;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IOrderService
{
    Task<ServiceReply<bool>> PlaceOrder( int userId, string email );
    Task<ServiceReply<List<OrderOverviewDto>?>> GetOrders( int userId );
    Task<ServiceReply<OrderDetailsDto?>> GetOrderDetails( int orderId );
}