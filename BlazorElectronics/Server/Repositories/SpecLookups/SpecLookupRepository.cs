using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.SpecLookups;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.SpecLookups;

public class SpecLookupRepository : DapperRepository, ISpecLookupRepository
{
    const string PROCEDURE_GET_SPEC_LOOKUPS = "Get_SpecLookups";

    public SpecLookupRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<SpecLookupsModel?> GetSpecLookupData()
    {
        return await TryQueryAsync( GetSpecLookupData );
    }
    static async Task<SpecLookupsModel?> GetSpecLookupData( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET_SPEC_LOOKUPS, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        return new SpecLookupsModel
        {
            GlobalSpecs = ( await multi.ReadAsync<int>() ).ToList(),
            SpecCategories = ( await multi.ReadAsync<SpecLookupCategoryModel>() ).ToList(),
            SpecLookups = ( await multi.ReadAsync<SpecLookupModel>() ).ToList(),
            SpecValues = ( await multi.ReadAsync<SpecLookupValueModel>() ).ToList()
        };
    }
}