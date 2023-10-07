using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Specs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Specs;

public class SpecDescrRepository : DapperRepository<SpecDescr>, ISpecDescrRepository
{
    const string STORED_PROCEDURE_GET_ALL = "Get_SpecDescrs";
    const string STORED_PROCEDURE_GET_BY_CATEGORY = "Get_SpecDescrsByCategory";

    const string COLUMN_NAME_SPEC_ID = "SpecId";
    const string COLUMN_NAME_SPEC_CATEGORY_ID = "SpecCategoryId";

    public SpecDescrRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public override async Task<IEnumerable<SpecDescr>?> GetAll()
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();

        var specDictionary = new Dictionary<int, SpecDescr>();
        const string splitOnColumns = $"{COLUMN_NAME_SPEC_ID},{COLUMN_NAME_SPEC_CATEGORY_ID}";

        IEnumerable<SpecDescr>? specs = await connection.QueryAsync<SpecDescr, SpecCategory, SpecDescr>
        ( STORED_PROCEDURE_GET_ALL, ( spec, category ) =>
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
    public override Task<SpecDescr?> GetById( int id ) { throw new NotImplementedException(); }
    public async Task<IEnumerable<SpecDescr>> GetByCategory( int categoryId )
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        var param = new { CategoryId = categoryId };
        return await connection.QueryAsync<SpecDescr>( STORED_PROCEDURE_GET_BY_CATEGORY, param, commandType: CommandType.StoredProcedure );
    }
}