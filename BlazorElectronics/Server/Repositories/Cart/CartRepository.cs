using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Cart;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Cart;

public class CartRepository : DapperRepository, ICartRepository
{
    const string PROCEDURE_GET_CART = "Get_Cart";
    const string PROCEDURE_UPDATE_CART = "Update_Cart";
    const string PROCEDURE_INSERT_TO_CART = "Insert_CartItem";
    const string PROCEDURE_UPDATE_QUANTITY = "Update_CartItem";
    const string PROCEDURE_DELETE = "Delete_CartItem";
    const string PROCEDURE_DELETE_CART = "Delete_Cart";

    public CartRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<IEnumerable<CartProductResponse>?> GetCart( int userId )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_ID, userId );

        return await TryQueryAsync( GetCartQuery, parameters );
    }
    public async Task<IEnumerable<CartProductResponse>?> UpdateCart( int userId, CartRequest request )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_ID, userId );

        var table = new DataTable();
        table.Columns.Add( TVP_COL_CART_PRODUCT_ID, typeof( int ) );
        table.Columns.Add( TVP_COL_CART_ITEM_QUANTITY, typeof( int ) );

        foreach ( CartItemDto item in request.Items )
        {
            DataRow row = table.NewRow();
            row[ TVP_COL_CART_PRODUCT_ID ] = item.ProductId;
            row[ TVP_COL_CART_ITEM_QUANTITY ] = item.Quantity;
            table.Rows.Add( row );
        }

        parameters.Add( PARAM_CART_REQUEST, table.AsTableValuedParameter( TVP_CART_ITEMS ) );

        return await TryQueryTransactionAsync( UpdateCartQuery, parameters );
    }
    public async Task<IEnumerable<CartProductResponse>?> InsertItem( int userId, CartItemDto item )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_ID, userId );
        parameters.Add( PARAM_PRODUCT_ID, item.ProductId );
        parameters.Add( PARAM_CART_QUANTITY, item.Quantity );

        return await TryQueryTransactionAsync( AddToCartQuery, parameters );
    }
    public async Task<IEnumerable<CartProductResponse>?> UpdateQuantity( int userId, CartItemDto item )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_ID, userId );
        parameters.Add( PARAM_PRODUCT_ID, item.ProductId );
        parameters.Add( PARAM_CART_QUANTITY, item.Quantity );

        return await TryQueryTransactionAsync( UpdateQuantityQuery, parameters );
    }
    public async Task<IEnumerable<CartProductResponse>?> DeleteFromCart( int userId, int productId )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_ID, userId );
        parameters.Add( PARAM_PRODUCT_ID, productId );

        return await TryQueryTransactionAsync( RemoveItemQuery, parameters );
    }
    public async Task<bool> DeleteCart( int userId )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_ID, userId );

        return await TryQueryTransactionAsync( ClearCartQuery, parameters );
    }

    static async Task<IEnumerable<CartProductResponse>?> GetCartQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? parameters )
    {
        return await connection.QueryAsync<CartProductResponse>( PROCEDURE_GET_CART, parameters, commandType: CommandType.StoredProcedure );
    }
    static async Task<IEnumerable<CartProductResponse>?> UpdateCartQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? parameters )
    {
        return await connection.QueryAsync<CartProductResponse>( PROCEDURE_UPDATE_CART, parameters, transaction, commandType: CommandType.StoredProcedure );
    }
    static async Task<IEnumerable<CartProductResponse>?> AddToCartQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? parameters )
    {
        return await connection.QueryAsync<CartProductResponse>( PROCEDURE_INSERT_TO_CART, parameters, transaction, commandType: CommandType.StoredProcedure );
    }
    static async Task<IEnumerable<CartProductResponse>?> UpdateQuantityQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? parameters )
    {
        return await connection.QueryAsync<CartProductResponse>( PROCEDURE_UPDATE_QUANTITY, parameters, transaction, commandType: CommandType.StoredProcedure );
    }
    static async Task<IEnumerable<CartProductResponse>?> RemoveItemQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? parameters )
    {
        return await connection.QueryAsync<CartProductResponse>( PROCEDURE_DELETE, parameters, transaction, commandType: CommandType.StoredProcedure );
    }
    static async Task<bool> ClearCartQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? parameters )
    {
        int rows = await connection.ExecuteAsync( PROCEDURE_DELETE_CART, parameters, commandType: CommandType.StoredProcedure );
        return rows > 0;
    }
}