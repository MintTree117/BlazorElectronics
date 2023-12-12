using System.Data;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.SpecLookups;
using BlazorElectronics.Shared.Specs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Data.Repositories;

public class SpecRepository : DapperRepository, ISpecRepository
{
    const string PROCEDURE_GET = "Get_Specs";
    const string PROCEDURE_GET_VIEW = "Get_SpecView";
    const string PROCEDURE_GET_EDIT = "Get_SpecEdit";
    const string PROCEDURE_INSERT = "Insert_Spec";
    const string PROCEDURE_UPDATE = "Update_Spec";
    const string PROCEDURE_DELETE = "Delete_Spec";

    public SpecRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public async Task<SpecsModel?> Get()
    {
        return await TryQueryAsync( GetQuery );
    }
    public async Task<IEnumerable<SpecModel>?> GetView()
    {
        return await TryQueryAsync( Query<SpecModel>, null, PROCEDURE_GET_VIEW );
    }
    public async Task<SpecEditModel?> GetEdit( int specId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_SPEC_ID, specId );
        return await TryQueryAsync( GetEditQuery, p );
    }
    public async Task<int> Insert( SpecEdit dto )
    {
        DynamicParameters p = GetInsertParams( dto );
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_INSERT );
    }
    public async Task<bool> Update( SpecEdit dto )
    {
        DynamicParameters p = GetUpdateParams( dto );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE );
    }
    public async Task<bool> Delete( int specId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_SPEC_ID, specId );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE );
    }
    
    static async Task<SpecsModel?> GetQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        return new SpecsModel
        {
            Specs = await multi.ReadAsync<SpecModel>(),
            SpecCategories = await multi.ReadAsync<SpecCategoryModel>(),
            SpecValues = await multi.ReadAsync<SpecValueModel>()
        };
    }
    static async Task<SpecEditModel?> GetEditQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? result = await connection.QueryMultipleAsync( PROCEDURE_GET_EDIT, dynamicParams, commandType: CommandType.StoredProcedure );

        if ( result is null )
            return null;
        
        return new SpecEditModel
        {
            Spec = await result.ReadSingleOrDefaultAsync<SpecModel>(),
            Categories = await result.ReadAsync<SpecCategoryModel>(),
            Values = await result.ReadAsync<SpecValueModel>()
        };
    }
    
    static DynamicParameters GetInsertParams( SpecEdit dto )
    {
        var parameters = new DynamicParameters();

        DataTable categoriesTable = GetPrimaryCategoriesTable( dto.PrimaryCategories );
        
        parameters.Add( PARAM_SPEC_NAME, dto.SpecName );
        parameters.Add( PARAM_IS_GLOBAL, dto.IsGlobal );
        parameters.Add( PARAM_SPEC_AVOID, dto.IsAvoid );
        parameters.Add( PARAM_CATEGORY_IDS, categoriesTable.AsTableValuedParameter( TVP_CATEGORY_IDS ) );

        DataTable valuesTable = GetStringValuesTable( dto.ValuesByIdAsString, COL_SPEC_VALUE_ID, COL_SPEC_VALUE );
        parameters.Add( PARAM_SPEC_VALUES, valuesTable.AsTableValuedParameter( TVP_SPEC_VALUES ) );
        
        return parameters;
    }
    static DynamicParameters GetUpdateParams( SpecEdit dto )
    {
        DynamicParameters parameters = GetInsertParams( dto );
        parameters.Add( PARAM_SPEC_ID, dto.SpecId );
        
        return parameters;
    }
}