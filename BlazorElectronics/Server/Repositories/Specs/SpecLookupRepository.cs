using System.Data;
using System.Text;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Specs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Specs;

public class SpecLookupRepository : DapperRepository, ISpecLookupRepository
{
    const string PROCEDURE_GET_SPEC_TABLE_META = "Get_SpecLookupDataRound1";

    public SpecLookupRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<SpecLookupsModel?> GetSpecLookupDataRound1()
    {
        SqlMapper.GridReader? multi = await TryQueryAsync( GetSpecLookupTableMetaQuery );

        if ( multi is null )
            return null;

        return new SpecLookupsModel
        {
            IntGlobalIds = await multi.ReadAsync() as IEnumerable<short>,
            StringGlobalIds = await multi.ReadAsync() as IEnumerable<short>,
            BoolGlobalIds = await multi.ReadAsync() as IEnumerable<short>,
            
            IntCategories = await multi.ReadAsync() as IEnumerable<SpecLookupSingleCategoryModel>,
            StringCategories = await multi.ReadAsync() as IEnumerable<SpecLookupSingleCategoryModel>,
            BoolCategories = await multi.ReadAsync() as IEnumerable<SpecLookupSingleCategoryModel>,
            
            IntNames = await multi.ReadAsync() as IEnumerable<SpecLookupNameModel>,
            StringNames = await multi.ReadAsync() as IEnumerable<SpecLookupNameModel>,
            BoolNames = await multi.ReadAsync() as IEnumerable<SpecLookupNameModel>,
            
            IntFilters = await multi.ReadAsync() as IEnumerable<SpecLookupIntFilterModel>,
            StringValues = await multi.ReadAsync() as IEnumerable<SpecLookupStringValueModel>,
            
            MultiTablesGlobal = await multi.ReadAsync() as IEnumerable<short>,
            MultiTableCategories = await multi.ReadAsync() as IEnumerable<SpecLookupMultiTableCategoryModel>,
            MultiTables = await multi.ReadAsync() as IEnumerable<SpecLookupMultiTableModel>
        };
    }
    public async Task<SpecLookupValuesModel?> GetSpecLookupDataRound2( IEnumerable<string> multiTableNames )
    {
        string dyanmicSql = await BuildSpecLookupDataRound2Query( multiTableNames );
        SqlMapper.GridReader? multi = await TryQueryAsync( GetDynamicSpecLookupsQuery, null, dyanmicSql );
        
        if ( multi is null )
            return null;

        var models = new List<IEnumerable<string>>();

        while ( !multi.IsConsumed )
        {
            models.Add( await multi.ReadAsync<string>() );
        }

        return new SpecLookupValuesModel()
        {
            ValuesByTable = models
        };
    }
    
    static async Task<SqlMapper.GridReader?> GetSpecLookupTableMetaQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryMultipleAsync( PROCEDURE_GET_SPEC_TABLE_META, commandType: CommandType.StoredProcedure );
    }
    static async Task<SqlMapper.GridReader?> GetDynamicSpecLookupsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryMultipleAsync( dynamicSql, commandType: CommandType.Text );
    }
    
    static async Task<string> BuildSpecLookupDataRound2Query( IEnumerable<string> multiTableNames )
    {
        var builder = new StringBuilder();

        await Task.Run( () =>
        {
            foreach ( string tableName in multiTableNames )
            {
                builder.Append( $"SELECT * FROM {tableName};" );
            }
        } );
        
        return builder.ToString();
    }
}