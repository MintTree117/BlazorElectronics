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
    const string QUERY_PARAM_VARIANT_ID = "@VariantId";
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

    public CartRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<int> CountCartItems( int userId )
    {
        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            return await connection.QuerySingleAsync<int>( STORED_PROCEDURE_COUNT_CART_ITEMS, commandType: CommandType.StoredProcedure ).ConfigureAwait( false );
        }
        catch ( SqlException e )
        {
            throw new RepositoryException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new RepositoryException( e.Message, e );
        }
    }
    public async Task<IEnumerable<CartItem>?> GetCartItems( int userId )
    {
        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            return await connection.QueryAsync<CartItem>( STORED_PROCEDURE_GET_CART_ITEMS, commandType: CommandType.StoredProcedure ).ConfigureAwait( false );
        }
        catch ( SqlException e )
        {
            throw new RepositoryException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new RepositoryException( e.Message, e );
        }
    }
    public async Task<IEnumerable<Product>?> GetCartProducts( List<int> productIds, List<int> variantIds )
    {
        var productIdBuilder = new StringBuilder();
        var variantIdBuilder = new StringBuilder();

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

        var productDictionary = new Dictionary<int, Product>();
        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add( QUERY_PARAM_PRODUCT_ID_LIST, productIdBuilder.ToString() );
        dynamicParameters.Add( QUERY_PARAM_VARIANT_ID_LIST, variantIdBuilder.ToString() );

        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
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
                dynamicParameters,
                splitOn: SqlConsts.COLUMN_PRODUCT_ID,
                commandType: CommandType.StoredProcedure );

            return productDictionary.Values;
        }
        catch ( SqlException e )
        {
            throw new RepositoryException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new RepositoryException( e.Message, e );
        }
    }
    public async Task<bool> InsertItems( List<CartItem> items )
    {
        SqlConnection? connection = null;
        DbTransaction? transaction = null;
        
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_CART_ITEMS, items );
        
        try
        {
            connection = await _dbContext.GetOpenConnection();
            transaction = await connection!.BeginTransactionAsync();
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }

        try
        {
            await connection.ExecuteAsync( STORED_PROCEDURE_ADD_CART_ITEMS, dynamicParams, commandType: CommandType.StoredProcedure, transaction: transaction ).ConfigureAwait( false );
            await transaction!.CommitAsync();
            await transaction.DisposeAsync();
            await connection!.CloseAsync();
            return true;
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
    }
    public async Task<bool> InsertItem( CartItem item )
    {
        SqlConnection? connection = null;
        DbTransaction? transaction = null;
        
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, item.UserId );
        dynamicParams.Add( QUERY_PARAM_PRODUCT_ID, item.ProductId );
        dynamicParams.Add( QUERY_PARAM_VARIANT_ID, item.VariantId );
        dynamicParams.Add( QUERY_PARAM_ITEM_QUANTITY, item.Quantity );

        try
        {
            connection = await _dbContext.GetOpenConnection();
            transaction = await connection!.BeginTransactionAsync();
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        
        try
        {
            await connection.ExecuteAsync( STORED_PROCEDURE_ADD_CART_ITEM, dynamicParams, commandType: CommandType.StoredProcedure, transaction: transaction ).ConfigureAwait( false );
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            await connection.CloseAsync();
            return true;
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
    }
    public async Task<bool> UpdateItemQuantity( CartItem item )
    {
        SqlConnection? connection = null;
        DbTransaction? transaction = null;
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, item.UserId );
        dynamicParams.Add( QUERY_PARAM_PRODUCT_ID, item.ProductId );
        dynamicParams.Add( QUERY_PARAM_VARIANT_ID, item.VariantId );
        dynamicParams.Add( QUERY_PARAM_ITEM_QUANTITY, item.Quantity );

        try
        {
            connection = await _dbContext.GetOpenConnection();
            transaction = await connection!.BeginTransactionAsync();
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }

        try
        {
            await connection.ExecuteAsync( STORED_PROCEDURE_UPDATE_CART_ITEM_QUANTITY, dynamicParams, commandType: CommandType.StoredProcedure, transaction: transaction ).ConfigureAwait( false );
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            await connection.CloseAsync();
            return true;
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
    }
    public async Task<bool> RemoveItem( CartItem item )
    {
        SqlConnection? connection = null;
        DbTransaction? transaction = null;
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, item.UserId );
        dynamicParams.Add( QUERY_PARAM_PRODUCT_ID, item.ProductId );
        dynamicParams.Add( QUERY_PARAM_VARIANT_ID, item.VariantId );
        dynamicParams.Add( QUERY_PARAM_ITEM_QUANTITY, item.Quantity );

        try
        {
            connection = await _dbContext.GetOpenConnection();
            transaction = await connection!.BeginTransactionAsync();
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }

        try
        {
            await connection.ExecuteAsync( STORED_PROCEDURE_REMOVE_CART_ITEM, dynamicParams, commandType: CommandType.StoredProcedure, transaction: transaction ).ConfigureAwait( false );
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            await connection.CloseAsync();
            return true;
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
    }
}