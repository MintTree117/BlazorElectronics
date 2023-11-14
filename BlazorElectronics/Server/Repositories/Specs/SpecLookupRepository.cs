using System.Data;
using System.Text;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Specs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Specs;

public class SpecLookupRepository : DapperRepository, ISpecLookupRepository
{
    const string PROCEDURE_GET_SPEC_LOOKUP_DATA = "Get_SpecLookupData";

    public SpecLookupRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<SpecLookupsModel?> GetSpecLookupData()
    {
        SqlMapper.GridReader? multi = await TryQueryAsync( GetSpecLookupData );

        if ( multi is null )
            return null;

        return new SpecLookupsModel
        {
            IntGlobalIds = await multi.ReadAsync() as IEnumerable<short>,
            StringGlobalIds = await multi.ReadAsync() as IEnumerable<short>,
            BoolGlobalIds = await multi.ReadAsync() as IEnumerable<short>,
            MultiGlobalIds = await multi.ReadAsync() as IEnumerable<short>,
            
            IntCategories = await multi.ReadAsync() as IEnumerable<SpecLookupCategoryModel>,
            StringCategories = await multi.ReadAsync() as IEnumerable<SpecLookupCategoryModel>,
            BoolCategories = await multi.ReadAsync() as IEnumerable<SpecLookupCategoryModel>,
            MultiCategories = await multi.ReadAsync() as IEnumerable<SpecLookupCategoryModel>,
            
            IntNames = await multi.ReadAsync() as IEnumerable<SpecLookupNameModel>,
            StringNames = await multi.ReadAsync() as IEnumerable<SpecLookupNameModel>,
            BoolNames = await multi.ReadAsync() as IEnumerable<SpecLookupNameModel>,
            MultiNames = await multi.ReadAsync() as IEnumerable<SpecLookupNameModel>,
            
            IntFilters = await multi.ReadAsync() as IEnumerable<SpecLookupIntFilterModel>,
            StringValues = await multi.ReadAsync() as IEnumerable<SpecLookupStringValueModel>,
            MultiValues = await multi.ReadAsync() as IEnumerable<SpecLookupStringValueModel>
        };
    }
    static async Task<SqlMapper.GridReader?> GetSpecLookupData( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryMultipleAsync( PROCEDURE_GET_SPEC_LOOKUP_DATA, commandType: CommandType.StoredProcedure );
    }
}