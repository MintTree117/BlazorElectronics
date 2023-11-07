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
    const string QUERY_PARAM_USER_ID = "@UserId";
    const string QUERY_PARAM_PRODUCT_ID = "@ProductId";
    const string QUERY_PARAM_VARIANT_ID = "@VariantSubId";
    const string QUERY_PARAM_ITEM_QUANTITY = "@Quantity";
    const string QUERY_PARAM_PRODUCT_ID_LIST = "@ProductIdList";
    const string QUERY_PARAM_VARIANT_ID_LIST = "@VariantIdList";
    const string QUERY_PARAM_CART_ITEMS = "@CartItems";
    
    const string STORED_PROCEDURE_COUNT_CART_ITEMS = "Count_CartItems";
    const string STORED_PROCEDURE_GET_CART_ITEMS = "Get_CartItems";
    const string STORED_PROCEDURE_GET_CART_PRODUCTS = "Get_CartProducts";
    const string STORED_PROCEDURE_ADD_CART_ITEMS = "Add_CartItems";
    const string STORED_PROCEDURE_ADD_CART_ITEM = "Add_CartItem";
    const string STORED_PROCEDURE_UPDATE_CART_ITEM_QUANTITY = "Update_CartItemQuantity";
    const string STORED_PROCEDURE_REMOVE_CART_ITEM = "Remove_CartItem";

    public CartRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<int?> CountCartItems( int userId )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, userId );

        return await TryQueryAsync( CountCartItemsQuery, dynamicParams );
    }
    public async Task<IEnumerable<CartItem>?> GetCartItems( int userId )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, userId );

        return await TryQueryAsync( GetCartItemsQuery, dynamicParams );
    }
    public async Task<IEnumerable<Product>?> GetCartProducts( List<int> productIds, List<int> variantIds )
    {
        var productIdBuilder = new StringBuilder();
        var variantIdBuilder = new StringBuilder();

        await BuildCartProductParams( productIdBuilder, variantIdBuilder, productIds, variantIds );

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add( QUERY_PARAM_PRODUCT_ID_LIST, productIdBuilder.ToString() );
        dynamicParameters.Add( QUERY_PARAM_VARIANT_ID_LIST, variantIdBuilder.ToString() );

        return await TryQueryAsync( GetCartProductsQuery, dynamicParameters );
    }
    public async Task<bool> AddCartItems( List<CartItem> items )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_CART_ITEMS, items );

        return await TryQueryTransactionAsync( AddCartItemsQuery, dynamicParams );
    }
    public async Task<bool> AddCartItem( CartItem item )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, item.UserId );
        dynamicParams.Add( QUERY_PARAM_PRODUCT_ID, item.ProductId );
        dynamicParams.Add( QUERY_PARAM_VARIANT_ID, item.VariantId );
        dynamicParams.Add( QUERY_PARAM_ITEM_QUANTITY, item.Quantity );

        return await TryQueryTransactionAsync( AddCartItemQuery, dynamicParams );
    }
    public async Task<bool> UpdateCartItemQuantity( CartItem item )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, item.UserId );
        dynamicParams.Add( QUERY_PARAM_PRODUCT_ID, item.ProductId );
        dynamicParams.Add( QUERY_PARAM_VARIANT_ID, item.VariantId );
        dynamicParams.Add( QUERY_PARAM_ITEM_QUANTITY, item.Quantity );

        return await TryQueryTransactionAsync( UpdateCartItemQuantityQuery, dynamicParams );
    }
    public async Task<bool> RemoveCartItem( CartItem item )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, item.UserId );
        dynamicParams.Add( QUERY_PARAM_PRODUCT_ID, item.ProductId );
        dynamicParams.Add( QUERY_PARAM_VARIANT_ID, item.VariantId );
        dynamicParams.Add( QUERY_PARAM_ITEM_QUANTITY, item.Quantity );
        
        return await TryQueryTransactionAsync( RemoveCartItemQuery, dynamicParams );
    }

    static async Task<int?> CountCartItemsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<int>( STORED_PROCEDURE_COUNT_CART_ITEMS, dynamicParams, commandType: CommandType.StoredProcedure ).ConfigureAwait( false );
    }
    static async Task<IEnumerable<CartItem>?> GetCartItemsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<CartItem>( STORED_PROCEDURE_GET_CART_ITEMS, dynamicParams, commandType: CommandType.StoredProcedure ).ConfigureAwait( false );
    }
    static async Task<IEnumerable<Product>?> GetCartProductsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        var productDictionary = new Dictionary<int, Product>();
        
        await connection.QueryAsync<Product, ProductVariant, Product>
        ( STORED_PROCEDURE_GET_CART_PRODUCTS, ( product, variant ) =>
            {
                if ( !productDictionary.TryGetValue( product.ProductId, out Product? productEntry ) )
                {
                    productEntry = product;
                    productDictionary.Add( productEntry.ProductId, productEntry );
                }
                if ( variant != null && productEntry.ProductVariants.Count <= 0 )
                    productEntry.ProductVariants.Add( variant );
                return productEntry;
            },
            dynamicParams,
            splitOn: COL_PRODUCT_ID,
            commandType: CommandType.StoredProcedure );

        return productDictionary.Values;
    }
    static async Task<bool> AddCartItemsQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int result = await connection.ExecuteAsync( STORED_PROCEDURE_ADD_CART_ITEMS, dynamicParams, commandType: CommandType.StoredProcedure, transaction: transaction ).ConfigureAwait( false );
        return result == 1;
    }
    static async Task<bool> AddCartItemQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int result = await connection.ExecuteAsync( STORED_PROCEDURE_ADD_CART_ITEM, dynamicParams, commandType: CommandType.StoredProcedure, transaction: transaction ).ConfigureAwait( false );
        return result == 1;
    }
    static async Task<bool> UpdateCartItemQuantityQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int result = await connection.ExecuteAsync( STORED_PROCEDURE_UPDATE_CART_ITEM_QUANTITY, dynamicParams, commandType: CommandType.StoredProcedure, transaction: transaction ).ConfigureAwait( false );
        return result == 1;
    }
    static async Task<bool> RemoveCartItemQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int result = await connection.ExecuteAsync( STORED_PROCEDURE_REMOVE_CART_ITEM, dynamicParams, commandType: CommandType.StoredProcedure, transaction: transaction ).ConfigureAwait( false );
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