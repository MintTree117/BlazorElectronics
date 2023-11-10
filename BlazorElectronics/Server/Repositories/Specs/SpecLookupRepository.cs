using System.Data;
using System.Text;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Specs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Specs;

public class SpecLookupRepository : DapperRepository, ISpecLookupRepository
{
    const string STORED_PROCEDURE_GET_SPEC_TABLE_META = "Get_SpecTableMeta";

    public SpecLookupRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<SpecLookupsModel?> GetSpecLookupMeta()
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
            
            IntFilters = await multi.ReadAsync() as IEnumerable<SpecLookupFilterModel>,
            StringFilters = await multi.ReadAsync() as IEnumerable<SpecLookupFilterModel>,
            
            MultiTablesGlobal = await multi.ReadAsync() as IEnumerable<short>,
            MultiTableCategories = await multi.ReadAsync() as IEnumerable<SpecLookupMultiTableCategoryModel>,
            MultiTables = await multi.ReadAsync() as IEnumerable<SpecLookupMultiTableModel>
        };
    }
    public async Task<SpecLookupValuesModel?> GetSpecLookupData( Dictionary<short, string> dynamicTableNamesById )
    {
        Dictionary<short, string>.KeyCollection tableIds = dynamicTableNamesById.Keys;

        string dyanmicSql = await BuildSpecDataQuery( tableIds, dynamicTableNamesById );
        SqlMapper.GridReader? multi = await TryQueryAsync( GetDynamicSpecLookupsQuery, null, dyanmicSql );

        if ( multi is null )
            return null;

        var specData = new SpecLookupValuesModel
        {
            DyanmicValuesByTableId = new Dictionary<int, IEnumerable<SpecLookupValueModel>?>()
        };

        await Task.Run( () =>
        {
            foreach ( short tableId in tableIds )
            {
                IEnumerable<SpecLookupValueModel>? data = multi.Read<SpecLookupValueModel>();
                specData.DyanmicValuesByTableId.TryAdd( tableId, data );
            }
        } );

        return specData;
    }
    
    static async Task<SqlMapper.GridReader?> GetSpecLookupTableMetaQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryMultipleAsync( STORED_PROCEDURE_GET_SPEC_TABLE_META, commandType: CommandType.StoredProcedure );
    }
    static async Task<SqlMapper.GridReader?> GetDynamicSpecLookupsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryMultipleAsync( dynamicSql, commandType: CommandType.Text );
    }
    
    static async Task<string> BuildSpecDataQuery( Dictionary<short, string>.KeyCollection tableIds, Dictionary<short, string> dynamicTableNamesById )
    {
        var builder = new StringBuilder();

        await Task.Run( () =>
        {
            foreach ( short tableId in tableIds )
            {
                builder.Append( $"SELECT * FROM {dynamicTableNamesById[ tableId ]};" );
            }
        } );
        
        return builder.ToString();
    }
}