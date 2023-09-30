using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Specs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Specs;

public class SpecRepository : DapperRepository, ISpecRepository
{
    const string STORED_PROCEDURE_GET_SPECS = "GetSpecs";
    const string STORED_PROCEDURE_GET_SPEC_LOOKUPS = "GetSpecLookups";

    const string COLUMN_NAME_SPEC_ID = "SpecId";
    const string COLUMN_NAME_SPEC_CATEGORY_ID = "SpecCategoryId";

    public SpecRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public async Task<IEnumerable<SpecDescr>?> GetSpecDescrs()
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();

        var specDictionary = new Dictionary<int, SpecDescr>();
        const string splitOnColumns = $"{COLUMN_NAME_SPEC_ID},{COLUMN_NAME_SPEC_CATEGORY_ID}";

        IEnumerable<SpecDescr>? specs = await connection.QueryAsync<SpecDescr, SpecCategory, SpecDescr>
        ( STORED_PROCEDURE_GET_SPECS, ( spec, category ) =>
            {
                if ( !specDictionary.TryGetValue( spec.SpecId, out SpecDescr? specEntry ) )
                {
                    specEntry = spec;
                    specDictionary.Add( specEntry.SpecId, specEntry );
                }
                if ( category != null && !specEntry.SpecCategories.Contains( category ) )
                    specEntry.SpecCategories.Add( category );
                return specEntry;
            },
            splitOn: splitOnColumns,
            commandType: CommandType.StoredProcedure );

        return specs;
    }
    public async Task<IEnumerable<SpecLookup>?> GetSpecLookups()
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        return await connection.QueryAsync<SpecLookup>( STORED_PROCEDURE_GET_SPEC_LOOKUPS, commandType: CommandType.StoredProcedure );
    }
}