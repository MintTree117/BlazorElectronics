using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Specs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Specs;

public class SpecRepository : ISpecRepository
{
    const string STORED_PROCEDURE_GET_SPECS = "GetSpecs";
    const string STORED_PROCEDURE_GET_SPEC_LOOKUPS = "GetSpecLookups";

    const string COLUMN_NAME_SPEC_ID = "SpecId";
    const string COLUMN_NAME_SPEC_CATEGORY_ID = "SpecCategoryId";
    
    readonly DapperContext _dbContext;

    public SpecRepository( DapperContext dapperContext ) { _dbContext = dapperContext; }
    
    public async Task<IEnumerable<Spec>?> GetSpecs()
    {
        await using SqlConnection connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        var specDictionary = new Dictionary<int, Spec>();

        IEnumerable<Spec>? specs = await connection.QueryAsync<Spec, SpecCategory, SpecFilter, Spec>
        ( STORED_PROCEDURE_GET_SPECS, ( spec, category, filter ) =>
            {
                if ( !specDictionary.TryGetValue( spec.SpecId, out Spec? specEntry ) )
                {
                    specEntry = spec;
                    specDictionary.Add( specEntry.SpecId, specEntry );
                }
                if ( category != null && !specEntry.SpecCategories.Contains( category ) )
                    specEntry.SpecCategories.Add( category );
                if ( filter != null && !specEntry.SpecFilters.Contains( filter ) )
                    specEntry.SpecFilters.Add( filter );
                return specEntry;
            },
            splitOn: $"{COLUMN_NAME_SPEC_ID},{COLUMN_NAME_SPEC_CATEGORY_ID}",
            commandType: CommandType.StoredProcedure );

        return specs;
    }
    public async Task<IEnumerable<SpecLookup>?> GetSpecLookups()
    {
        await using SqlConnection connection = _dbContext.CreateConnection();
        await connection.OpenAsync();
        return await connection.QueryAsync<SpecLookup>(
            STORED_PROCEDURE_GET_SPEC_LOOKUPS, CommandType.StoredProcedure );
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}