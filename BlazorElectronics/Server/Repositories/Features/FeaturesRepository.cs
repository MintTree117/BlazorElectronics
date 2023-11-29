using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Features;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Features;

public class FeaturesRepository : DapperRepository, IFeaturesRepository
{
    const string PROCEDURE_GET_VIEW = "Get_FeaturesView";
    const string PROCEDURE_GET_PRODUCT_EDIT = "Get_FeaturedProductEdit";
    const string PROCEDURE_INSERT_PRODUCT = "Insert_FeaturedProduct";
    const string PROCEDURE_INSERT_DEAL = "Insert_FeaturedDeal";
    const string PROCEDURE_UPDATE_PRODUCT = "Update_FeaturedProduct";
    const string PROCEDURE_DELETE_PRODUCT = "Delete_FeaturedProduct";
    const string PROCEDURE_DELETE_DEAL = "Delete_FeaturedDeal";
    
    public FeaturesRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<FeaturesResponse?> GetView()
    {
        return await TryQueryAsync( GetViewQuery );
    }
    public async Task<bool> InsertFeaturedProduct( FeaturedProductDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_PRODUCT_ID, dto.ProductId );
        parameters.Add( PARAM_FEATURE_IMAGE_URL, dto.ImageUrl );

        return await TryQueryTransactionAsync( InsertProductQuery, parameters );
    }
    public async Task<bool> InsertFeaturedDeal( int productId )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_PRODUCT_ID, productId );
        
        return await TryQueryTransactionAsync( InsertDealQuery, parameters );
    }
    public async Task<FeaturedProductDto?> GetFeaturedProductEdit( int productId )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_PRODUCT_ID, productId );

        return await TryQueryAsync( GetProductEditQuery );
    }
    public async Task<bool> UpdateFeaturedProduct( FeaturedProductDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_PRODUCT_ID, dto.ProductId );
        parameters.Add( PARAM_FEATURE_IMAGE_URL, dto.ImageUrl );

        return await TryQueryTransactionAsync( UpdateProductQuery, parameters );
    }
    public async Task<bool> DeleteFeaturedProduct( int productId )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_PRODUCT_ID, productId );

        return await TryQueryTransactionAsync( DeleteProductQuery, parameters );
    }
    public async Task<bool> DeleteFeaturedDeal( int productId )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_PRODUCT_ID, productId );

        return await TryQueryTransactionAsync( DeleteDealQuery, parameters );
    }
    
    static async Task<FeaturesResponse?> GetViewQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET_VIEW, dynamicParams, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;
        
        IEnumerable<FeaturedProductDto>? products = await multi.ReadAsync<FeaturedProductDto>();
        IEnumerable<FeaturedDealDto>? deals = await multi.ReadAsync<FeaturedDealDto>();

        return new FeaturesResponse
        {
            FeaturedProducts = products is not null ? products.ToList() : new List<FeaturedProductDto>(),
            FeaturedDeals = deals is not null ? deals.ToList() : new List<FeaturedDealDto>()
        };
    }
    static async Task<FeaturedProductDto?> GetProductEditQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleOrDefaultAsync<FeaturedProductDto>( PROCEDURE_GET_PRODUCT_EDIT, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<bool> InsertProductQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int rowsAffected = await connection.ExecuteAsync( PROCEDURE_INSERT_PRODUCT, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
    }
    static async Task<bool> InsertDealQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int rowsAffected = await connection.ExecuteAsync( PROCEDURE_INSERT_DEAL, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
    }
    static async Task<bool> UpdateProductQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int rowsAffected = await connection.ExecuteAsync( PROCEDURE_UPDATE_PRODUCT, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
    }
    static async Task<bool> DeleteProductQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int rowsAffected = await connection.ExecuteAsync( PROCEDURE_DELETE_PRODUCT, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
    }
    static async Task<bool> DeleteDealQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int rowsAffected = await connection.ExecuteAsync( PROCEDURE_DELETE_DEAL, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
    }
}