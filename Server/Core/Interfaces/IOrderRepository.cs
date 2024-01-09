using BlazorElectronics.Server.Core.Models.Orders;
using BlazorElectronics.Shared.Orders;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IOrderRepository
{
    Task<bool> InsertOrder( OrderModel orderModel );
    Task<IEnumerable<OrderOverviewDto>?> GetOrders( int userId );
    Task<OrderDetailsModel?> GetOrderDetails( int orderId );
}