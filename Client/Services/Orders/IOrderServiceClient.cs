using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Orders;

namespace BlazorElectronics.Client.Services.Orders;

public interface IOrderServiceClient
{
    Task<ServiceReply<string?>> PlaceOrder();
    Task<ServiceReply<List<OrderOverviewDto>?>> GetOrders();
    Task<ServiceReply<OrderDetailsDto?>> GetOrderDetails( int orderId );
}