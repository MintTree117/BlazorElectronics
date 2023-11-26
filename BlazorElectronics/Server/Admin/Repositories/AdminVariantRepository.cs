using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.Admin.Models;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Admin.Variants;
using BlazorElectronics.Shared.Enums;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Admin.Repositories;

public sealed class AdminVariantRepository : _AdminRepository, IAdminVariantRepository
{
    const string PROCEDURE_GET_VIEW = "Get_VariantsView";
    const string PROCEDURE_GET_EDIT = "Get_VariantEdit";
    const string PROCEDURE_INSERT = "Insert_Variant";
    const string PROCEDURE_UPDATE = "Update_Variant";
    const string PROCEDURE_DELETE = "Delete_Variant";
    
    public AdminVariantRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<List<VariantViewDto>?> GetView()
    {
        return await TryQueryAsync( GetViewQuery );
    }
    public async Task<VariantEditDto?> GetEdit( int id )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_VARIANT_ID, id );

        return await TryQueryAsync( GetEditQuery, parameters );
    }
    public async Task<int> Insert( VariantAddDto dto )
    {
        DynamicParameters parameters = GetInsertParameters( dto.PrimaryCategoryId, dto.VariantName, dto.VariantValues );
        return await TryQueryTransactionAsync( InsertQuery, parameters );
    }
    public async Task<bool> Update( VariantEditDto dto )
    {
        DynamicParameters parameters = GetUpdateParameters( dto );
        return await TryQueryTransactionAsync( UpdateQuery, parameters );
    }
    public async Task<bool> Delete( int id )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_VARIANT_ID, id );
        
        return await TryQueryTransactionAsync( DeleteQuery, parameters );
    }

    static async Task<List<VariantViewDto>?> GetViewQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        IEnumerable<VariantViewDto>? result = await connection.QueryAsync<VariantViewDto>( PROCEDURE_GET_VIEW, commandType: CommandType.StoredProcedure );
        return result?.ToList();
    }
    static async Task<VariantEditDto?> GetEditQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET_EDIT, dynamicParams, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;
        
        var view = await multi.ReadFirstOrDefaultAsync<VariantViewDto>();

        if ( view is null )
            return null;
        
        IEnumerable<AdminVariantValueModel>? values = await multi.ReadAsync<AdminVariantValueModel>();
        
        return new VariantEditDto
        {
            VariantId = view.VariantId,
            VariantName = view.VariantName,
            PrimaryCategoryId = view.PrimaryCategoryId,
            VariantValues = ConvertVariantValuesToString( values )
        };
    }
    static async Task<int> InsertQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.ExecuteScalarAsync<int>( PROCEDURE_INSERT, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
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
    
    static DynamicParameters GetInsertParameters( PrimaryCategory primaryCategory, string name, string values )
    {
        var parameters = new DynamicParameters();
        
        DataTable valuesTableParam = GetStringValuesTable( values, TVP_COL_VARIANT_VALUE_ID, TVP_COL_VARIANT_VALUE );
        parameters.Add( PARAM_VARIANT_VALUES, valuesTableParam.AsTableValuedParameter( TVP_VARIANT_VALUES ) );
        parameters.Add( PARAM_VARIANT_NAME, name );
        parameters.Add( PARAM_CATEGORY_PRIMARY_ID, primaryCategory );
        
        return parameters;
    }
    static DynamicParameters GetUpdateParameters( VariantEditDto dto )
    {
        DynamicParameters parameters = GetInsertParameters( dto.PrimaryCategoryId, dto.VariantName, dto.VariantValues );
        parameters.Add( PARAM_VARIANT_ID, dto.VariantId );
        return parameters;
    }

    static string ConvertVariantValuesToString( IEnumerable<AdminVariantValueModel>? models )
    {
        if ( models is null )
            return string.Empty;

        IEnumerable<string> sortedValues = models
            .OrderBy( x => x.VariantValueId )
            .Select( x => x.VariantValue );

        return string.Join( ",", sortedValues );
    }
}