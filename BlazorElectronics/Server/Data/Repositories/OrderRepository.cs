using System.Data;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Orders;
using BlazorElectronics.Shared.Orders;
using BlazorElectronics.Shared.Promos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Data.Repositories;

public sealed class OrderRepository : DapperRepository, IOrderRepository
{
    const string PROCEDURE_GET_CART_ORDER = "Get_CartOrder";
    const string PROCEDURE_INSERT = "Insert_Order";
    const string PROCEDURE_GET_ORDERS = "Get_Orders";
    const string PROCEDURE_GET_ORDER_DETAILS = "Get_OrderDetails";
    
    public OrderRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<CartOrderModel?> GetCartOrder( int userId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );

        return await TryQueryAsync( GetCartOrderQuery, p, PROCEDURE_GET_CART_ORDER );
    }
    public async Task<bool> InsertOrder( OrderModel orderModel )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, orderModel.UserId );
        p.Add( PARAM_ORDER_DATE, orderModel.OrderDate );
        p.Add( PARAM_TOTAL_PRICE, orderModel.TotalPrice );
        p.Add( PARAM_ORDER_ITEMS, GetOrderItemsTable( orderModel.OrderItems ).AsTableValuedParameter( TVP_ORDER_ITEMS ) );
        p.Add( PARAM_ORDER_PROMOS, GetOrderPromosTable( orderModel.PromoCodes ).AsTableValuedParameter( TVP_ORDER_PROMOS ) );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_INSERT );
    }
    public async Task<IEnumerable<OrderOverviewDto>?> GetOrders( int userId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );

        return await TryQueryAsync( Query<OrderOverviewDto>, p, PROCEDURE_GET_ORDERS );
    }
    public async Task<OrderDetailsModel?> GetOrderDetails( int orderId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_ORDER_ID, orderId );

        return await TryQueryAsync( GetOrderDetailsQuery, p, null );
    }

    async Task<CartOrderModel?> GetCartOrderQuery( SqlConnection connection, string? sql, DynamicParameters? p )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET_CART_ORDER, p, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        return new CartOrderModel
        {
            Items = await multi.ReadAsync<CartOrderItemModel>(),
            Promos = await multi.ReadAsync<PromoCodeDto>()
        };
    }
    async Task<OrderDetailsModel?> GetOrderDetailsQuery( SqlConnection connection, string? sql, DynamicParameters? p )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET_ORDER_DETAILS, p, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        return new OrderDetailsModel
        {
            Meta = await multi.ReadFirstOrDefaultAsync<OrderDetailsMetaModel>(),
            Items = await multi.ReadAsync<OrderDetailsProductDto>()
        };
    }

    static DataTable GetOrderItemsTable( List<OrderItemModel> items )
    {
        DataTable table = new();
        table.Columns.Add( COL_PRODUCT_ID, typeof( int ) );
        table.Columns.Add( COL_QUANTITY, typeof( int ) );
        table.Columns.Add( COL_TOTAL_PRICE, typeof( decimal ) );

        foreach ( OrderItemModel m in items )
        {
            DataRow row = table.NewRow();
            row[ COL_PRODUCT_ID ] = m.ProductId;
            row[ COL_QUANTITY ] = m.Quantity;
            row[ COL_TOTAL_PRICE ] = m.TotalPrice;

            table.Rows.Add( row );
        }

        return table;
    }
    static DataTable GetOrderPromosTable( List<PromoCodeDto> promos )
    {
        DataTable table = new();
        table.Columns.Add( COL_PROMO_CODE, typeof( string ) );
        table.Columns.Add( COL_PROMO_DISCOUNT, typeof( decimal ) );

        foreach ( PromoCodeDto p in promos )
        {
            DataRow row = table.NewRow();
            row[ COL_PROMO_CODE ] = p.PromoCode;
            row[ COL_PROMO_DISCOUNT ] = p.Discount;

            table.Rows.Add( row );
        }

        return table;
    }
}