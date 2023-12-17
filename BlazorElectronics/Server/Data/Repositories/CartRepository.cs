using System.Data;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Shared.Cart;
using Dapper;

namespace BlazorElectronics.Server.Data.Repositories;

public class CartRepository : DapperRepository, ICartRepository
{
    const string PROCEDURE_GET_CART = "Get_Cart";
    const string PROCEDURE_UPDATE_CART = "Update_Cart";
    const string PROCEDURE_INSERT_TO_CART = "Insert_CartItem";
    const string PROCEDURE_UPDATE_QUANTITY = "Update_CartItem";
    const string PROCEDURE_DELETE = "Delete_CartItem";
    const string PROCEDURE_DELETE_CART = "Delete_Cart";

    public CartRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<IEnumerable<CartProductDto>?> GetCart( int userId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        return await TryQueryAsync( Query<CartProductDto>, p, PROCEDURE_GET_CART );
    }
    public async Task<IEnumerable<CartProductDto>?> UpdateCart( int userId, CartRequestDto requestDto )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );

        DataTable table = new();
        table.Columns.Add( COL_PRODUCT_ID, typeof( int ) );
        table.Columns.Add( COL_CART_ITEM_QUANTITY, typeof( int ) );

        foreach ( CartItemDto item in requestDto.Items )
        {
            DataRow row = table.NewRow();
            row[ COL_PRODUCT_ID ] = item.ProductId;
            row[ COL_CART_ITEM_QUANTITY ] = item.Quantity;
            table.Rows.Add( row );
        }

        p.Add( PARAM_CART_ITEMS, table.AsTableValuedParameter( TVP_CART_ITEMS ) );
        
        return await TryQueryTransactionAsync( QueryTransaction<CartProductDto>, p, PROCEDURE_UPDATE_CART );
    }
    public async Task<IEnumerable<CartProductDto>?> InsertItem( int userId, CartItemDto itemDto )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_PRODUCT_ID, itemDto.ProductId );
        p.Add( PARAM_CART_QUANTITY, itemDto.Quantity );

        return await TryQueryTransactionAsync( QueryTransaction<CartProductDto>, p, PROCEDURE_INSERT_TO_CART );
    }
    public async Task<IEnumerable<CartProductDto>?> UpdateQuantity( int userId, CartItemDto itemDto )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_PRODUCT_ID, itemDto.ProductId );
        p.Add( PARAM_CART_QUANTITY, itemDto.Quantity );

        return await TryQueryTransactionAsync( QueryTransaction<CartProductDto>, p, PROCEDURE_UPDATE_QUANTITY );
    }
    public async Task<IEnumerable<CartProductDto>?> DeleteFromCart( int userId, int productId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_PRODUCT_ID, productId );

        return await TryQueryTransactionAsync( QueryTransaction<CartProductDto>, p, PROCEDURE_DELETE );
    }
    public async Task<bool> DeleteCart( int userId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE_CART );
    }
}