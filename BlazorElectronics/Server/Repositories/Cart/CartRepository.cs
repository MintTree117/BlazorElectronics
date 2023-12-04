using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Cart;
using Dapper;

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
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        return await TryQueryAsync( Query<CartProductResponse>, p, PROCEDURE_GET_CART );
    }
    public async Task<IEnumerable<CartProductResponse>?> UpdateCart( int userId, CartRequest request )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );

        DataTable table = new();
        table.Columns.Add( TVP_COL_CART_PRODUCT_ID, typeof( int ) );
        table.Columns.Add( TVP_COL_CART_ITEM_QUANTITY, typeof( int ) );

        foreach ( CartItemDto item in request.Items )
        {
            DataRow row = table.NewRow();
            row[ TVP_COL_CART_PRODUCT_ID ] = item.ProductId;
            row[ TVP_COL_CART_ITEM_QUANTITY ] = item.Quantity;
            table.Rows.Add( row );
        }

        p.Add( PARAM_CART_ITEMS, table.AsTableValuedParameter( TVP_CART_ITEMS ) );
        
        return await TryQueryTransactionAsync( QueryTransaction<CartProductResponse>, p, PROCEDURE_UPDATE_CART );
    }
    public async Task<IEnumerable<CartProductResponse>?> InsertItem( int userId, CartItemDto item )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_PRODUCT_ID, item.ProductId );
        p.Add( PARAM_CART_QUANTITY, item.Quantity );

        return await TryQueryTransactionAsync( QueryTransaction<CartProductResponse>, p, PROCEDURE_INSERT_TO_CART );
    }
    public async Task<IEnumerable<CartProductResponse>?> UpdateQuantity( int userId, CartItemDto item )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_PRODUCT_ID, item.ProductId );
        p.Add( PARAM_CART_QUANTITY, item.Quantity );

        return await TryQueryTransactionAsync( QueryTransaction<CartProductResponse>, p, PROCEDURE_UPDATE_QUANTITY );
    }
    public async Task<IEnumerable<CartProductResponse>?> DeleteFromCart( int userId, int productId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_PRODUCT_ID, productId );

        return await TryQueryTransactionAsync( QueryTransaction<CartProductResponse>, p, PROCEDURE_DELETE );
    }
    public async Task<bool> DeleteCart( int userId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE_CART );
    }
}