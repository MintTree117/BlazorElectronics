using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Promos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Data.Repositories;

public class CartRepository : DapperRepository, ICartRepository
{
    const string PROCEDURE_UPDATE_CART = "Update_Cart";
    const string PROCEDURE_INSERT_TO_CART = "InsertOrUpdate_CartItem";
    const string PROCEDURE_DELETE = "Delete_CartItem";
    const string PROCEDURE_DELETE_CART = "Delete_Cart";

    public CartRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public async Task<CartDto?> UpdateCart( int userId, List<CartItemDto> items )
    {
        DynamicParameters p = GetCartItemsParameters( userId, items );
        return await TryQueryTransactionAsync( UpdateCartQuery, p );
    }
    public async Task<bool> InsertOrUpdateItem( int userId, CartItemDto itemDto )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_PRODUCT_ID, itemDto.ProductId );
        p.Add( PARAM_CART_QUANTITY, itemDto.ItemQuantity );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_INSERT_TO_CART );
    }
    public async Task<bool> DeleteItem( int userId, int productId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_PRODUCT_ID, productId );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE );
    }
    public async Task<bool> DeleteCart( int userId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE_CART );
    }

    static async Task<CartDto?> UpdateCartQuery( SqlConnection connection, DbTransaction transaction, string? sql, DynamicParameters? p )
    {
        SqlMapper.GridReader multi = await connection.QueryMultipleAsync( PROCEDURE_UPDATE_CART, p, transaction, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        return new CartDto
        {
            Products = ( await multi.ReadAsync<CartProductDto>() ).ToList(),
            PromoCodes = ( await multi.ReadAsync<PromoCodeDto>() ).ToList()
        };
    }

    static DynamicParameters GetCartItemsParameters( int userId, List<CartItemDto> items )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_CART_ITEMS, GetCartItemsTable( items ).AsTableValuedParameter( TVP_CART_ITEMS ) );

        return p;
    }
    static DataTable GetCartItemsTable( List<CartItemDto> items )
    {
        DataTable table = new();
        table.Columns.Add( COL_PRODUCT_ID, typeof( int ) );
        table.Columns.Add( COL_CART_ITEM_QUANTITY, typeof( int ) );

        foreach ( CartItemDto d in items )
        {
            DataRow row = table.NewRow();
            row[ COL_PRODUCT_ID ] = d.ProductId;
            row[ COL_CART_ITEM_QUANTITY ] = d.ItemQuantity;

            table.Rows.Add( row );
        }

        return table;
    }
}