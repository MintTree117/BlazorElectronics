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

    public async Task<SpecLookupTableMetaModel?> GetSpecTableMeta()
    {
        return await TryQueryAsync( GetSpecLookupTableMetaQuery );
    }
    public async Task<SpecLookupDataModel?> GetSpecLookupData( SpecLookupTableMetaModel meta )
    {
        //string dynamicSql = await BuildSpecDataQuery( dynamicTableMeta );
        //return await TryQueryAsync( GetSpecLookupTableMetaQuery );
        return null;
    }
    public async Task<DynamicSpecLookups?> GetDynamicSpecLookups( Dictionary<short, string> tableNames )
    {

        SqlMapper.GridReader? multi = await TryQueryAsync( GetDynamicSpecLookupsQuery, null, "" );

        if ( multi == null )
            return null;
        
        var lookups = new DynamicSpecLookups();
        List<short> tableIds = tableNames.Keys.ToList();
        
        await Task.Run( () =>
        {
            int count = 0;
            while ( !multi.IsConsumed )
            {
                if ( count >= tableNames.Count )
                    break;

                int tableId = tableIds[ count ];
                IEnumerable<DynamicSpecLookup>? data = multi.Read<DynamicSpecLookup>();
                lookups.SpecLookups.TryAdd( tableId, data );
                
                count++;
            }
        } );

        return lookups;
    }
    
    static async Task<SpecLookupTableMetaModel?> GetSpecLookupTableMetaQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( STORED_PROCEDURE_GET_SPEC_TABLE_META, commandType: CommandType.StoredProcedure );

        return await Task.Run( () => new SpecLookupTableMetaModel
        {
            ExplicitIntGlobalIds = multi.Read() as IEnumerable<int>,
            ExplicitStringGlobalIds = multi.Read() as IEnumerable<int>,
            ExplicitIntCategories = multi.Read() as IEnumerable<ExplicitProductSpecCategory>,
            ExplicitStringCategories = multi.Read() as IEnumerable<ExplicitProductSpecCategory>,
            ExplicitIntNames = multi.Read() as IEnumerable<ExplicitProductSpecName>,
            ExplicitStringNames = multi.Read() as IEnumerable<ExplicitProductSpecName>,
            DyanmicGlobalIds = multi.Read() as IEnumerable<int>,
            DynamicCategories = multi.Read() as IEnumerable<DynamicSpecTableCategory>,
            DynamicTableMeta = multi.Read() as IEnumerable<DynamicSpecTableMeta>
        } );
    }
    static async Task<SqlMapper.GridReader?> GetDynamicSpecLookupsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryMultipleAsync( dynamicSql, commandType: CommandType.Text );
    }
    
    static async Task<string> BuildSpecDataQuery( List<DynamicSpecTableMeta> tableMeta )
    {
        var builder = new StringBuilder();

        await Task.Run( () =>
        {
            foreach ( DynamicSpecTableMeta meta in tableMeta )
            {
                builder.Append( $"SELECT * FROM {meta.LookupTableName};" );
            }
        } );
        
        return builder.ToString();
    }
}