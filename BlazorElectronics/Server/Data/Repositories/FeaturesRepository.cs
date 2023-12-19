using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Shared.Features;
using Dapper;

namespace BlazorElectronics.Server.Data.Repositories;

public class FeaturesRepository : DapperRepository, IFeaturesRepository
{
    const string PROCEDURE_GET_DEALS = "Get_FeaturedDeals";
    const string PROCEDURE_GET_FEATURES = "Get_Features";
    const string PROCEDURE_GET_FEATURE = "Get_Feature";
    const string PROCEDURE_INSERT_FEATURE = "Insert_Feature";
    const string PROCEDURE_UPDATE_FEATURE = "Update_Feature";
    const string PROCEDURE_DELETE_FEATURE = "Delete_Feature";

    public FeaturesRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<IEnumerable<FeatureDealDto>?> GetDeals( int rows, int offset )
    {
        DynamicParameters p = new();
        p.Add( PARAM_ROWS, rows );
        p.Add( PARAM_OFFSET, offset );

        return await TryQueryAsync( Query<FeatureDealDto>, p, PROCEDURE_GET_DEALS );
    }
    public async Task<IEnumerable<FeatureDto>?> GetFeatures()
    {
        return await TryQueryAsync( Query<FeatureDto>, null, PROCEDURE_GET_FEATURES );
    }
    public async Task<FeatureDtoEditDto?> GetFeatureEdit( int featureId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_FEATURE_ID, featureId );

        return await TryQueryAsync( QuerySingleOrDefault<FeatureDtoEditDto?>, p, PROCEDURE_GET_FEATURE );
    }
    public async Task<int> InsertFeature( FeatureDtoEditDto dto )
    {
        DynamicParameters p = GetFeatureInsertParams( dto );
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_INSERT_FEATURE );
    }
    public async Task<bool> UpdateFeature( FeatureDtoEditDto dto )
    {
        DynamicParameters p = GetFeatureUpdateParams( dto );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE_FEATURE );
    }
    public async Task<bool> DeleteFeature( int featureId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_FEATURE_ID, featureId );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE_FEATURE );
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
}