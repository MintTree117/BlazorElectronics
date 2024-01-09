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
    const string PROCEDURE_GET_CART = "Get_Cart";
    const string PROCEDURE_UPDATE_CART = "Update_Cart";
    const string PROCEDURE_INSERT_UPDATE_ITEM = "InsertOrUpdate_CartItem";
    const string PROCEDURE_DELETE_ITEM = "Delete_CartItem";
    const string PROCEDURE_DELETE_CART = "Delete_Cart";
    const string PROCEDURE_INSERT_PROMO = "Insert_CartPromo";
    const string PROCEDURE_DELETE_PROMO = "Delete_CartPromo";

    public CartRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<CartDto?> Get( int userId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );

        return await TryQueryAsync( GetCartQuery, p );
    }
    public async Task<CartDto?> Update( int userId, List<CartItemDto> items )
    {
        DynamicParameters p = GetCartItemsParameters( userId, items );
        return await TryQueryTransactionAsync( UpdateCartQuery, p );
    }
    public async Task<bool> InsertOrUpdateItem( int userId, CartItemDto itemDto )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_PRODUCT_ID, itemDto.ProductId );
        p.Add( PARAM_QUANTITY, itemDto.Quantity );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_INSERT_UPDATE_ITEM );
    }
    public async Task<bool> DeleteItem( int userId, int productId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_PRODUCT_ID, productId );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE_ITEM );
    }
    public async Task<bool> DeleteCart( int userId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE_CART );
    }
    public async Task<PromoCodeDto?> InsertCartPromo( int userId, string code )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_PROMO_CODE, code );

        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<PromoCodeDto?>, p, PROCEDURE_INSERT_PROMO );
    }
    public async Task<bool> DeleteCartPromo( int userId, int promoId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_PROMO_ID, promoId );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE_PROMO );
    }

    static async Task<CartDto?> GetCartQuery( SqlConnection connection, string? sql, DynamicParameters? p )
    {
        SqlMapper.GridReader multi = await connection.QueryMultipleAsync( PROCEDURE_GET_CART, p, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        return new CartDto
        {
            Products = ( await multi.ReadAsync<CartProductDto>() ).ToList(),
            PromoCodes = ( await multi.ReadAsync<PromoCodeDto>() ).ToList()
        };
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
        table.Columns.Add( COL_QUANTITY, typeof( int ) );

        foreach ( CartItemDto d in items )
        {
            DataRow row = table.NewRow();
            row[ COL_PRODUCT_ID ] = d.ProductId;
            row[ COL_QUANTITY ] = d.Quantity;

            table.Rows.Add( row );
        }

        return table;
    }
}