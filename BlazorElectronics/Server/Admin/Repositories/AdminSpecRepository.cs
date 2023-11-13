using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Admin.SpecsMulti;
using BlazorElectronics.Shared.Admin.SpecsSingle;
using Dapper;

namespace BlazorElectronics.Server.Admin.Repositories;

public class AdminSpecRepository : _AdminRepository, IAdminSpecRepository
{
    static readonly string[] PROCEDURES_ADD_SINGLE = { "Add_SpecLookupSingleInt", "Add_SpecLookupSingleInt", "Add_SpecLookupSingleInt" };
    static readonly string[] PROCEDURES_UPDATE_SINGLE = { "Update_SpecLookupSingleInt", "Update_SpecLookupSingleInt", "Update_SpecLookupSingleInt" };
    static readonly string[] PROCEDURES_REMOVE_SINGLE = { "Remove_SpecLookupSingleInt", "Remove_SpecLookupSingleInt", "Remove_SpecLookupSingleInt" };
    
    const string PROCEDURE_UPDATE_MULTI_TABLE = "Update_SpecLookupMulti";

    const string PARAM_PRIMARY_CATEGORIES = "@PrimaryCategories";
    const string PARAM_IS_GLOBAL = "@IsGlobal";
    const string PARAM_TABLE_ID = "@TableId";
    const string PARAM_SPEC_ID = "@SpecId";
    const string PARAM_SPEC_NAME = "@SpecName";
    const string PARAM_FILTER_VALUES = "@FilterValue";
    const string PARAM_SPEC_VALUES = "@SpecValues";

    public AdminSpecRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public async Task<bool> AddSpecSingle( AddUpdateSpecSingleDto dto )
    {
        string procedure = GetSingleProcedure( dto.SpecType, PROCEDURES_ADD_SINGLE );
        DynamicParameters parameters = GetAddUpdateSingleParams( dto, false );

        return await ExecuteAdminTransaction( procedure, parameters );
    }
    public async Task<bool> UpdateSpecSingle( AddUpdateSpecSingleDto dto )
    {
        string procedure = GetSingleProcedure( dto.SpecType, PROCEDURES_UPDATE_SINGLE );
        DynamicParameters parameters = GetAddUpdateSingleParams( dto, false );

        return await ExecuteAdminTransaction( procedure, parameters );
    }
    public async Task<bool> RemoveSpecSingle( RemoveSpecSingleDto dto )
    {
        string procedure = GetSingleProcedure( dto.SpecType, PROCEDURES_REMOVE_SINGLE );
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_ID, dto.SpecId );

        return await ExecuteAdminTransaction( procedure, parameters );
    }
    
    public async Task<bool> UpdateSpecMultiTable( UpdateSpecMultiDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_TABLE_ID, dto.TableId );
        parameters.Add( PARAM_SPEC_VALUES, dto.MultiValues );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, dto.PrimaryCategories );
        parameters.Add( PARAM_IS_GLOBAL, dto.IsGlobal );

        return await ExecuteAdminTransaction( PROCEDURE_UPDATE_MULTI_TABLE, parameters );
    }

    static DynamicParameters GetAddUpdateSingleParams( AddUpdateSpecSingleDto dto, bool isUpdate )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_NAME, dto.SpecName );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, dto.PrimaryCategories );
        parameters.Add( PARAM_IS_GLOBAL, dto.IsGlobal );

        if ( isUpdate )
            parameters.Add( PARAM_SPEC_ID, dto.SpecId );

        switch ( dto.SpecType )
        {
            case SingleSpecLookupType.INT:
                parameters.Add( PARAM_FILTER_VALUES, dto.ValuesById );
                break;
            case SingleSpecLookupType.STRING:
                parameters.Add( PARAM_SPEC_VALUES, dto.ValuesById );
                break;
            case SingleSpecLookupType.BOOL:
            default: return parameters;
        }

        return parameters;
    }
    static string GetSingleProcedure( SingleSpecLookupType type, IReadOnlyList<string> procedures )
    {
        return procedures[ ( int ) type ];
    }
}