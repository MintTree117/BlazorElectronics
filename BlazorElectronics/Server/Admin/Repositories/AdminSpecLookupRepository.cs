using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.Admin.Models.SpecLookups;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Admin.SpecLookups;
using BlazorElectronics.Shared.SpecLookups;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Admin.Repositories;

public class AdminSpecLookupRepository : _AdminRepository, IAdminSpecLookupRepository
{
    const string PROCEDURE_GET_VIEW = "Get_SpecLookupView";
    const string PROCEDURE_GET_EDIT = "Get_SpecLookupEdit";
    const string PROCEDURE_INSERT = "Insert_SpecLookup";
    const string PROCEDURE_UPDATE = "Update_SpecLookup";
    const string PROCEDURE_DELETE = "Delete_SpecLookup";

    public AdminSpecLookupRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<List<SpecLookupViewDto>?> GetView()
    {
        return await TryQueryAsync( GetViewQuery );
    }
    public async Task<SpecLookupEditDto?> GetEdit( int specId )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_ID, specId );
        
        return await TryQueryAsync( GetEditQuery, parameters );
    }
    public async Task<int> Insert( SpecLookupEditDto dto )
    {
        DynamicParameters parameters = GetInsertParams( dto );

        return await TryQueryTransactionAsync( InsertQuery, parameters );
    }
    public async Task<bool> Update( SpecLookupEditDto dto )
    {
        DynamicParameters parameters = GetUpdateParams( dto );

        return await TryQueryTransactionAsync( UpdateQuery, parameters );
    }
    public async Task<bool> Delete( int specId )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_ID, specId );

        return await TryQueryTransactionAsync( DeleteQuery, parameters );
    }

    static async Task<List<SpecLookupViewDto>?> GetViewQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        IEnumerable<SpecLookupViewDto>? result = await connection.QueryAsync<SpecLookupViewDto>( PROCEDURE_GET_VIEW, dynamicParams, commandType: CommandType.StoredProcedure );
        return result?.ToList();
    }
    static async Task<SpecLookupEditDto?> GetEditQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? result = await connection.QueryMultipleAsync( PROCEDURE_GET_EDIT, dynamicParams, commandType: CommandType.StoredProcedure );

        if ( result is null )
            return null;

        var specModel = await result.ReadSingleOrDefaultAsync<AdminSpecModel>();
        IEnumerable<int>? specCategories = await result.ReadAsync<int>();
        IEnumerable<AdminSpecValueModel>? specValues = await result.ReadAsync<AdminSpecValueModel>();
        
        return new SpecLookupEditDto
        {
            SpecId = specModel.SpecId,
            SpecName = specModel.SpecName,
            IsGlobal = await result.ReadSingleOrDefaultAsync<bool>(),
            PrimaryCategoriesAsString = ConvertPrimaryCategoriesToString( specCategories ),
            ValuesByIdAsString = ConvertSpecValuesToString( specValues )
        };
    }
    static async Task<int> InsertQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleOrDefaultAsync( PROCEDURE_INSERT, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
    }
    static async Task<bool> UpdateQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int rowsAffected = await connection.ExecuteAsync( PROCEDURE_UPDATE, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
    }
    static async Task<bool> DeleteQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int rowsAffected = await connection.ExecuteAsync( PROCEDURE_DELETE, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
    }
    
    static DynamicParameters GetInsertParams( SpecLookupEditDto dto )
    {
        var parameters = new DynamicParameters();

        DataTable categoriesTable = GetPrimaryCategoriesTable( dto.PrimaryCategoriesAsString );
        
        parameters.Add( PARAM_SPEC_NAME, dto.SpecName );
        parameters.Add( PARAM_IS_GLOBAL, dto.IsGlobal );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, categoriesTable.AsTableValuedParameter( TVP_PRIMARY_CATEGORIES ) );

        DataTable valuesTable = GetStringValuesTable( dto.ValuesByIdAsString, TVP_COL_SPEC_ID, TVP_COL_SPEC_VALUE );
        parameters.Add( PARAM_SPEC_VALUES, valuesTable.AsTableValuedParameter( TVP_SPEC_VALUES ) );
        
        return parameters;
    }
    static DynamicParameters GetUpdateParams( SpecLookupEditDto dto )
    {
        DynamicParameters parameters = GetInsertParams( dto );
        parameters.Add( PARAM_SPEC_ID, dto.SpecId );
        
        return parameters;
    }
}