using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Admin.SpecsMulti;
using BlazorElectronics.Shared.Admin.SpecsSingle;
using Dapper;

namespace BlazorElectronics.Server.Admin.Repositories;

public class AdminSpecLookupRepository : _AdminRepository, IAdminSpecLookupRepository
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
    
    public async Task<bool> AddSpecSingle( AddSpecSingleDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_NAME, dto.SpecName );
        parameters.Add( PARAM_FILTER_VALUES, dto.FilterValuesById );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, dto.PrimaryCategories );
        parameters.Add( PARAM_IS_GLOBAL, dto.IsGlobal );

        return await ExecuteAdminTransaction( GetSingleProcedure( dto.SpecType, PROCEDURE_ADD_SINGLE ), parameters );
    }
    public async Task<bool> UpdateSpecSingle( UpdateSpecSingleDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_ID, dto.SpecId );
        parameters.Add( PARAM_SPEC_NAME, dto.SpecName );
        parameters.Add( PARAM_FILTER_VALUES, dto.FilterValuesById );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, dto.PrimaryCategories );
        parameters.Add( PARAM_IS_GLOBAL, dto.IsGlobal );

        return await ExecuteAdminTransaction( GetSingleProcedure( dto.SpecType, PROCEDURE_UPDATE_SINGLE ), parameters );
    }
    public async Task<bool> RemoveSpecSingle( RemoveSpecSingleDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_ID, dto.SpecId );

        return await ExecuteAdminTransaction( GetSingleProcedure( dto.SpecType, PROCEDURE_DELETE_SINGLE ), parameters );
    }
    
    public async Task<bool> AddSpecMultiTable( AddSpecMultiDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_TABLE_NAME, dto.TableName );
        parameters.Add( PARAM_MULTI_VALUES, dto.MultiValues );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, dto.PrimaryCategories );
        parameters.Add( PARAM_IS_GLOBAL, dto.IsGlobal );

        return await ExecuteAdminTransaction( PROCEDURE_ADD_MULTI_TABLE, parameters );
    }
    public async Task<bool> UpdateSpecMultiTable( UpdateSpecMultiDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_TABLE_ID, dto.TableId );
        parameters.Add( PARAM_TABLE_NAME, dto.TableName );
        parameters.Add( PARAM_MULTI_VALUES, dto.MultiValues );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, dto.PrimaryCategories );
        parameters.Add( PARAM_IS_GLOBAL, dto.IsGlobal );

        return await ExecuteAdminTransaction( PROCEDURE_UPDATE_MULTI_TABLE, parameters );
    }
    public async Task<bool> RemoveSpecMultiTable( RemoveSpecMultiDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_TABLE_ID, dto.TableId );
        parameters.Add( PARAM_TABLE_NAME, dto.TableName );

        return await ExecuteAdminTransaction( PROCEDURE_DELETE_MULTI_TABLE, parameters );
    }

    static string GetSingleProcedure( SingleSpecLookupType type, IReadOnlyList<string> procedures )
    {
        return procedures[ ( int ) type ];
    }
}