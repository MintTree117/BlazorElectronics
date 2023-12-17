using System.Data;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Shared.Features;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Data.Repositories;

public class FeaturesRepository : DapperRepository, IFeaturesRepository
{
    const string PROCEDURE_GET = "Get_Features";
    const string PROCEDURE_GET_FEATURES = "Get_FeaturesView";
    const string PROCEDURE_GET_DEALS = "Get_FeaturedDealsView";
    const string PROCEDURE_GET_FEATURE = "Get_Feature";
    const string PROCEDURE_GET_DEAL = "Get_FeaturedDeal";
    const string PROCEDURE_INSERT_FEATURE = "Insert_Feature";
    const string PROCEDURE_INSERT_DEAL = "Insert_FeaturedDeal";
    const string PROCEDURE_UPDATE_FEATURE = "Update_Feature";
    const string PROCEDURE_UPDATE_DEAL = "Update_FeaturedDeal";
    const string PROCEDURE_DELETE_FEATURE = "Delete_Feature";
    const string PROCEDURE_DELETE_DEAL = "Delete_FeaturedDeal";

    public FeaturesRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<FeaturesReplyDto?> Get()
    {
        return await TryQueryAsync( GetQuery );
    }
    public async Task<IEnumerable<FeatureDto>?> GetFeatures()
    {
        return await TryQueryAsync( Query<FeatureDto>, null, PROCEDURE_GET_FEATURES );
    }
    public async Task<IEnumerable<FeaturedDealDto>?> GetDeals()
    {
        return await TryQueryAsync( Query<FeaturedDealDto>, null, PROCEDURE_GET_DEALS );
    }
    public async Task<FeatureDtoEditDto?> GetFeature( int featureId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_FEATURE_ID, featureId );

        return await TryQueryAsync( QuerySingleOrDefault<FeatureDtoEditDto?>, p, PROCEDURE_GET_FEATURE );
    }
    public async Task<FeaturedDealDtoEditDto?> GetDeal( int productId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_ID, productId );

        return await TryQueryAsync( QuerySingleOrDefault<FeaturedDealDtoEditDto?>, p, PROCEDURE_GET_DEAL );
    }
    public async Task<int> InsertFeature( FeatureDtoEditDto dto )
    {
        DynamicParameters p = GetFeatureInsertParams( dto );
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_INSERT_FEATURE );
    }
    public async Task<int> InsertDeal( FeaturedDealDtoEditDto dto )
    {
        DynamicParameters p = GetDealParams( dto );
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_INSERT_DEAL );
    }
    public async Task<bool> UpdateFeature( FeatureDtoEditDto dto )
    {
        DynamicParameters p = GetFeatureUpdateParams( dto );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE_FEATURE );
    }
    public async Task<bool> UpdateDeal( FeaturedDealDtoEditDto dto )
    {
        DynamicParameters p = GetDealParams( dto );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE_DEAL );
    }
    public async Task<bool> DeleteFeature( int featureId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_FEATURE_ID, featureId );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE_FEATURE );
    }
    public async Task<bool> DeleteDeal( int productId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_ID, productId );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE_DEAL );
    }

    static async Task<FeaturesReplyDto?> GetQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET, dynamicParams, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        return new FeaturesReplyDto
        {
            Features = ( await multi.ReadAsync<FeatureDto>() ).ToList(),
            Deals = ( await multi.ReadAsync<FeaturedDealDto>() ).ToList()
        };
    }

    static DynamicParameters GetFeatureInsertParams( FeatureDto dto )
    {
        DynamicParameters p = new();
        p.Add( PARAM_FEATURE_NAME, dto.Name );
        p.Add( PARAM_FEATURE_URL, dto.Url );
        p.Add( PARAM_FEATURE_IMAGE, dto.Image );
        return p;
    }
    static DynamicParameters GetFeatureUpdateParams( FeatureDto dto )
    {
        DynamicParameters p = GetFeatureInsertParams( dto );
        p.Add( PARAM_FEATURE_ID, dto.FeatureId );
        return p;
    }
    static DynamicParameters GetDealParams( FeaturedDealDto dto )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_ID, dto.ProductId );
        p.Add( PARAM_FEATURE_IMAGE, dto.Image );
        return p;
    }
}