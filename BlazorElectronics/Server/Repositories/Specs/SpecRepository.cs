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
    
    public async Task<Dictionary<string, Spec>?> GetSpecs()
    {
        await using SqlConnection connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        var specDictionary = new Dictionary<string, Spec>();

        IEnumerable<Spec?>? specs = await connection.QueryAsync<Spec, SpecCategory, SpecFilter, Spec?>
        ( STORED_PROCEDURE_GET_SPECS, ( spec, category, filter ) =>
            {
                if ( spec?.SpecName == null )
                    return null;
                if ( !specDictionary.TryGetValue( spec.SpecName, out Spec? specEntry ) )
                {
                    specEntry = spec;
                    specDictionary.Add( specEntry.SpecName, specEntry );
                }
                if ( category != null && !specEntry.SpecCategories.Contains( category.CategoryId ) )
                    specEntry.SpecCategories.Add( category.CategoryId );
                if ( filter != null && !specEntry.SpecFilters.Contains( filter.FilterId ) )
                    specEntry.SpecFilters.Add( filter.FilterId );
                return specEntry;
            },
            splitOn: $"{COLUMN_NAME_SPEC_ID},{COLUMN_NAME_SPEC_CATEGORY_ID}",
            commandType: CommandType.StoredProcedure );

        return specs == null ? null : specDictionary;
    }
    public Task<Dictionary<int, List<object>>?> GetSpecLookups() { throw new NotImplementedException(); }
}