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

    public async Task<SpecLookupMetaModel?> GetSpecLookupMeta()
    {
        SqlMapper.GridReader? multi = await TryQueryAsync( GetSpecLookupTableMetaQuery );

        if ( multi is null )
            return null;

        return new SpecLookupMetaModel
        {
            IntGlobalIds = await multi.ReadAsync() as IEnumerable<short>,
            StringGlobalIds = await multi.ReadAsync() as IEnumerable<short>,
            BoolGlobalIds = await multi.ReadAsync() as IEnumerable<short>,
            
            IntCategories = await multi.ReadAsync() as IEnumerable<RawSpecCategoryModel>,
            StringCategories = await multi.ReadAsync() as IEnumerable<RawSpecCategoryModel>,
            BoolCategories = await multi.ReadAsync() as IEnumerable<RawSpecCategoryModel>,
            
            IntNames = await multi.ReadAsync() as IEnumerable<RawSpecNameModel>,
            StringNames = await multi.ReadAsync() as IEnumerable<RawSpecNameModel>,
            BoolNames = await multi.ReadAsync() as IEnumerable<RawSpecNameModel>,
            
            IntFilters = await multi.ReadAsync() as IEnumerable<IntFilterModel>,
            StringValues = await multi.ReadAsync() as IEnumerable<StringSpecValueModel>,
            
            DyanmicGlobalTableIds = await multi.ReadAsync() as IEnumerable<short>,
            DynamicTableCategories = await multi.ReadAsync() as IEnumerable<DynamicSpecTableCategoryModel>,
            DynamicTables = await multi.ReadAsync() as IEnumerable<DynamicSpecTableMetaModel>
        };
    }
    public async Task<DynamicSpecLookupValuesModel?> GetSpecLookupData( Dictionary<short, string> dynamicTableNamesById )
    {
        Dictionary<short, string>.KeyCollection tableIds = dynamicTableNamesById.Keys;

        string dyanmicSql = await BuildSpecDataQuery( tableIds, dynamicTableNamesById );
        SqlMapper.GridReader? multi = await TryQueryAsync( GetDynamicSpecLookupsQuery, null, dyanmicSql );

        if ( multi is null )
            return null;

        var specData = new DynamicSpecLookupValuesModel
        {
            DyanmicValuesByTableId = new Dictionary<int, IEnumerable<DynamicSpecValueModel>?>()
        };

        await Task.Run( () =>
        {
            foreach ( short tableId in tableIds )
            {
                IEnumerable<DynamicSpecValueModel>? data = multi.Read<DynamicSpecValueModel>();
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