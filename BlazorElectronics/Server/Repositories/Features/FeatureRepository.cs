using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Features;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Features;

public class FeatureRepository : DapperRepository, IFeatureRepository
{
    const string PROCEDURE_GET_FEATURES = "Get_Features";
    
    public FeatureRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<IEnumerable<Feature>?> GetFeatures()
    {
        return await TryQueryAsync( GetFeaturesQuery );
    }

    static async Task<IEnumerable<Feature>?> GetFeaturesQuery( SqlConnection connection, string? sql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<Feature>( sql, PROCEDURE_GET_FEATURES, commandType: CommandType.StoredProcedure );
    }
}