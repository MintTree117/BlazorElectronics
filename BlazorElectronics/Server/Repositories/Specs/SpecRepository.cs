using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Specs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Specs;

public class SpecRepository : DapperRepository, ISpecRepository
{
    const string STORED_PROCEDURE_GET_ALL_DESCRS = "Get_SpecDescrs";
    const string STORED_PROCEDURE_GET_DESCRS_BY_CATEGORY = "Get_SpecDescrsByCategory";
    const string STORED_PROCEDURE_GET_ALL_LOOKUPS = "Get_SpecLookups";
    const string STORED_PROCEDURE_GET_LOOKUPS_BY_CATEGORY = "Get_SpecLookupsByCategory";

    const string LOOKUP_ID_COLUMN = "LookupId";
    const string COLUMN_NAME_SPEC_ID = "SpecId";
    const string COLUMN_NAME_SPEC_CATEGORY_ID = "SpecCategoryId";

    public SpecRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public async Task<IEnumerable<SpecDescr>?> GetAllSpecDescrs()
    {
        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();

            var specDictionary = new Dictionary<int, SpecDescr>();
            const string splitOnColumns = $"{COLUMN_NAME_SPEC_ID},{COLUMN_NAME_SPEC_CATEGORY_ID}";

            IEnumerable<SpecDescr>? specs = await connection.QueryAsync<SpecDescr, SpecCategory, SpecDescr>
            ( STORED_PROCEDURE_GET_ALL_DESCRS, ( spec, category ) =>
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
        catch ( SqlException e )
        {
            throw new RepositoryException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new RepositoryException( e.Message, e );
        }
    }
    public async Task<IEnumerable<SpecDescr>?> GetSpecDescrsByCategory( int categoryId )
    {
        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            var param = new { CategoryId = categoryId };
            return await connection.QueryAsync<SpecDescr>( STORED_PROCEDURE_GET_DESCRS_BY_CATEGORY, param, commandType: CommandType.StoredProcedure );
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
    public async Task<IEnumerable<SpecLookup>?> GetAllSpecLookups()
    {
        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            return await connection.QueryAsync<SpecLookup>( STORED_PROCEDURE_GET_ALL_LOOKUPS, commandType: CommandType.StoredProcedure );
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
    public async Task<IEnumerable<SpecLookup>?> GetSpecLookupsByCategory( int categoryId )
    {
        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            var param = new { CategoryId = categoryId };
            return await connection.QueryAsync<SpecLookup>( STORED_PROCEDURE_GET_LOOKUPS_BY_CATEGORY, param, commandType: CommandType.StoredProcedure );
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
}