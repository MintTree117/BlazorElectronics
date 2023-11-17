using System.Data;
using System.Data.Common;
using System.Text;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Cart;
using BlazorElectronics.Server.Models.Products;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Cart;

public class CartRepository : DapperRepository, ICartRepository
{
    const string PROCEDURE_COUNT_CART_ITEMS = "Count_CartItems";
    const string PROCEDURE_GET_CART_ITEMS = "Get_CartItems";
    const string PROCEDURE_GET_CART_PRODUCTS = "Get_CartProducts";
    const string PROCEDURE_ADD_CART_ITEMS = "Add_CartItems";
    const string PROCEDURE_ADD_CART_ITEM = "Add_CartItem";
    const string PROCEDURE_UPDATE_CART_ITEM_QUANTITY = "Update_CartItemQuantity";
    const string PROCEDURE_REMOVE_CART_ITEM = "Remove_CartItem";

    public CartRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public async Task<int?> CountCartItems( int userId )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( PARAM_USER_ID, userId );

        return await TryQueryAsync( CountCartItemsQuery, dynamicParams );
    }
    public async Task<IEnumerable<CartItem>?> GetCartItems( int userId )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( PARAM_USER_ID, userId );

        return await TryQueryAsync( GetCartItemsQuery, dynamicParams );
    }
    public async Task<IEnumerable<CartProductModel>?> GetCartProducts( List<int> productIds, List<int> variantIds )
    {
        var productIdBuilder = new StringBuilder();
        var variantIdBuilder = new StringBuilder();

        await BuildCartProductParams( productIdBuilder, variantIdBuilder, productIds, variantIds );

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add( PARAM_CART_PRODUCT_IDS, productIdBuilder.ToString() );
        dynamicParameters.Add( PARAM_CART_VARIANT_IDS, variantIdBuilder.ToString() );

        return await TryQueryAsync( GetCartProductsQuery, dynamicParameters );
    }
    public async Task<bool> AddCartItems( List<CartItem> items )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( PARAM_CART_ITEMS, items );

        return await TryQueryTransactionAsync( AddCartItemsQuery, dynamicParams );
    }
    public async Task<bool> AddCartItem( CartItem item )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( PARAM_USER_ID, item.UserId );
        dynamicParams.Add( PARAM_PRODUCT_ID, item.ProductId );
        dynamicParams.Add( PARAM_VARIANT_ID, item.VariantId );
        dynamicParams.Add( PARAM_ITEM_QUANTITY, item.Quantity );

        return await TryQueryTransactionAsync( AddCartItemQuery, dynamicParams );
    }
    public async Task<bool> UpdateCartItemQuantity( CartItem item )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( PARAM_USER_ID, item.UserId );
        dynamicParams.Add( PARAM_PRODUCT_ID, item.ProductId );
        dynamicParams.Add( PARAM_VARIANT_ID, item.VariantId );
        dynamicParams.Add( PARAM_ITEM_QUANTITY, item.Quantity );

        return await TryQueryTransactionAsync( UpdateCartItemQuantityQuery, dynamicParams );
    }
    public async Task<bool> RemoveCartItem( CartItem item )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( PARAM_USER_ID, item.UserId );
        dynamicParams.Add( PARAM_PRODUCT_ID, item.ProductId );
        dynamicParams.Add( PARAM_VARIANT_ID, item.VariantId );
        dynamicParams.Add( PARAM_ITEM_QUANTITY, item.Quantity );
        
        return await TryQueryTransactionAsync( RemoveCartItemQuery, dynamicParams );
    }

    static async Task<int?> CountCartItemsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<int>( PROCEDURE_COUNT_CART_ITEMS, dynamicParams, commandType: CommandType.StoredProcedure ).ConfigureAwait( false );
    }
    static async Task<IEnumerable<CartItem>?> GetCartItemsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<CartItem>( PROCEDURE_GET_CART_ITEMS, dynamicParams, commandType: CommandType.StoredProcedure ).ConfigureAwait( false );
    }
    static async Task<IEnumerable<CartProductModel>?> GetCartProductsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        var productDictionary = new Dictionary<int, CartProductModel>();

        return productDictionary.Values;
    }
    static async Task<bool> AddCartItemsQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int result = await connection.ExecuteAsync( PROCEDURE_ADD_CART_ITEMS, dynamicParams, commandType: CommandType.StoredProcedure, transaction: transaction ).ConfigureAwait( false );
        return result == 1;
    }
    static async Task<bool> AddCartItemQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int result = await connection.ExecuteAsync( PROCEDURE_ADD_CART_ITEM, dynamicParams, commandType: CommandType.StoredProcedure, transaction: transaction ).ConfigureAwait( false );
        return result == 1;
    }
    static async Task<bool> UpdateCartItemQuantityQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int result = await connection.ExecuteAsync( PROCEDURE_UPDATE_CART_ITEM_QUANTITY, dynamicParams, commandType: CommandType.StoredProcedure, transaction: transaction ).ConfigureAwait( false );
        return result == 1;
    }
    static async Task<bool> RemoveCartItemQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int result = await connection.ExecuteAsync( PROCEDURE_REMOVE_CART_ITEM, dynamicParams, commandType: CommandType.StoredProcedure, transaction: transaction ).ConfigureAwait( false );
        return result == 1;
    }
    
    async Task BuildCartProductParams( StringBuilder productIdBuilder, StringBuilder variantIdBuilder, List<int> productIds, List<int> variantIds )
    {
        await Task.Run( () =>
        {
            for ( int i = 0; i < productIds.Count; i++ )
            {
                productIdBuilder.Append( $"{productIds[ i ]}" );
                variantIdBuilder.Append( $"{variantIds[ i ]}" );

                if ( i >= productIds.Count - 1 )
                    continue;

                productIdBuilder.Append( "," );
                variantIdBuilder.Append( "," );
            }
        } );
    }
}