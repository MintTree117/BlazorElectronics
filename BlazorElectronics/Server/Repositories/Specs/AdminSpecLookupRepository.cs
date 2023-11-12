using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Inbound.Admin.SpecLookups;
using Dapper;

namespace BlazorElectronics.Server.Repositories.Specs;

public class AdminSpecLookupRepository : AdminRepository, IAdminSpecLookupRepository
{
    static readonly string[] PROCEDURE_ADD_SINGLE = new [] { "", "", "" };
    static readonly string[] PROCEDURE_UPDATE_SINGLE = new[] { "", "", "" };
    static readonly string[] PROCEDURE_DELETE_SINGLE = new[] { "", "", "" };
    
    const string PROCEDURE_ADD_MULTI_TABLE = "";
    const string PROCEDURE_UPDATE_MULTI_TABLE = "";
    const string PROCEDURE_DELETE_MULTI_TABLE = "";
    
    const string PARAM_PRIMARY_CATEGORIES = "";
    const string PARAM_IS_GLOBAL = "";
    const string PARAM_TABLE_ID = "@TableId";
    const string PARAM_TABLE_NAME = "@TableName";
    const string PARAM_MULTI_VALUES = "";
    const string PARAM_SPEC_ID = "@SpecId";
    const string PARAM_SPEC_NAME = "@SpecName";
    const string PARAM_FILTER_VALUES = "@FilterValue";

    public AdminSpecLookupRepository( DapperContext dapperContext ) : base( dapperContext ) { }


    public async Task<bool> AddSpecSingle( SingleSpecLookupType specType, string specName, Dictionary<int, object>? filterValuesById, List<int>? primaryCategories, bool isGlobal )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_NAME, specName );
        parameters.Add( PARAM_FILTER_VALUES, filterValuesById );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, primaryCategories );
        parameters.Add( PARAM_IS_GLOBAL, isGlobal );

        return await ExecuteAdminTransaction( GetSingleProcedure( specType, PROCEDURE_ADD_SINGLE ), parameters );
    }
    public async Task<bool> UpdateSpecSingle( SingleSpecLookupType specType, int specId, string specName, Dictionary<int, object>? filterValuesById, List<int> primaryCategories, bool isGlobal )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_ID, specId );
        parameters.Add( PARAM_SPEC_NAME, specName );
        parameters.Add( PARAM_FILTER_VALUES, filterValuesById );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, primaryCategories );
        parameters.Add( PARAM_IS_GLOBAL, isGlobal );

        return await ExecuteAdminTransaction( GetSingleProcedure( specType, PROCEDURE_UPDATE_SINGLE ), parameters );
    }
    public async Task<bool> DeleteSpecSingle( SingleSpecLookupType specType, int specId )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_ID, specId );

        return await ExecuteAdminTransaction( GetSingleProcedure( specType, PROCEDURE_DELETE_SINGLE ), parameters );
    }
    public async Task<bool> AddSpecMultiTable( string tableName, List<string> multiValues, List<int> primaryCategories, bool isGlobal )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_TABLE_NAME, tableName );
        parameters.Add( PARAM_MULTI_VALUES, multiValues );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, primaryCategories );
        parameters.Add( PARAM_IS_GLOBAL, isGlobal );

        return await ExecuteAdminTransaction( PROCEDURE_ADD_MULTI_TABLE, parameters );
    }
    public async Task<bool> UpdateSpecMultiTable( int tableId, string tableName, List<string> multiValues, List<int> primaryCategories, bool isGlobal )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_TABLE_ID, tableId );
        parameters.Add( PARAM_TABLE_NAME, tableName );
        parameters.Add( PARAM_MULTI_VALUES, multiValues );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, primaryCategories );
        parameters.Add( PARAM_IS_GLOBAL, isGlobal );

        return await ExecuteAdminTransaction( PROCEDURE_UPDATE_MULTI_TABLE, parameters );
    }
    public async Task<bool> DeleteSpecMultiTable( int tableId, string tableName )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_TABLE_ID, tableId );
        parameters.Add( PARAM_TABLE_NAME, tableName );

        return await ExecuteAdminTransaction( PROCEDURE_DELETE_MULTI_TABLE, parameters );
    }

    static string GetSingleProcedure( SingleSpecLookupType type, IReadOnlyList<string> procedures )
    {
        return procedures[ ( int ) type ];
    }
}