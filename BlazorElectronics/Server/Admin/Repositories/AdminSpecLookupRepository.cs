using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.Admin.Models;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Admin.Specs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Admin.Repositories;

public class AdminSpecLookupRepository : _AdminRepository, IAdminSpecLookupRepository
{
    const string PROCEDURE_GET_SPECS_VIEW = "Get_SpecLookupView";
    static readonly string[] PROCEDURES_GET_EDIT = { "Get_SpecLookupSingleIntEdit", "Get_SpecLookupSingleStringEdit", "Get_SpecLookupSingleBoolEdit", "Get_SpecLookupMultiStringEdit" };
    static readonly string[] PROCEDURES_INSERT = { "Insert_SpecLookupSingleInt", "Insert_SpecLookupSingleString", "Insert_SpecLookupSingleBool", "Insert_SpecLookupMultiString" };
    static readonly string[] PROCEDURES_UPDATE = { "Update_SpecLookupSingleInt", "Update_SpecLookupSingleString", "Update_SpecLookupSingleBool", "Update_SpecLookupMultiString" };
    static readonly string[] PROCEDURES_DELETE = { "Delete_SpecLookupSingleInt", "Delete_SpecLookupSingleString", "Delete_SpecLookupSingleBool", "Delete_SpecLookupMultiString" };

    public AdminSpecLookupRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public async Task<SpecsViewDto?> GetView()
    {
        return await TryQueryAsync( GetViewQuery );
    }
    public async Task<SpecLookupEditDto?> GetEdit( SpecLookupGetEditDto dto )
    {
        string procedure = GetProcedure( dto.SpecType, PROCEDURES_GET_EDIT );
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_ID, dto.SpedId );

        DapperQueryDelegate<SpecLookupEditDto> method = dto.SpecType switch
        {
            SpecLookupType.INT => GetEditIntQuery,
            SpecLookupType.BOOL => GetEditBoolQuery,
            SpecLookupType.STRING => GetEditStringQuery,
            SpecLookupType.MULTI => GetEditStringQuery,
            _ => throw new Exception( "Invalid SpecLookupType!" )
        };

        SpecLookupEditDto? result = await TryQueryAsync( method, parameters, procedure );

        if ( result is null )
            return null;
        
        result.SpecType = dto.SpecType;
        return result;
    }
    public async Task<int> Insert( SpecLookupEditDto dto )
    {
        string procedure = GetProcedure( dto.SpecType, PROCEDURES_INSERT );
        DynamicParameters parameters = GetInsertParams( dto );

        return await TryQueryTransactionAsync( InsertQuery, parameters, procedure );
    }
    public async Task<bool> Update( SpecLookupEditDto dto )
    {
        string procedure = GetProcedure( dto.SpecType, PROCEDURES_UPDATE );
        DynamicParameters parameters = GetUpdateParams( dto );

        return await TryQueryTransactionAsync( UpdateQuery, parameters, procedure );
    }
    public async Task<bool> Delete( SpecLookupRemoveDto dto )
    {
        string procedure = GetProcedure( dto.SpecType, PROCEDURES_DELETE );
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_ID, dto.SpecId );

        return await TryQueryTransactionAsync( DeleteQuery, parameters, procedure );
    }
    
    static async Task<SpecsViewDto?> GetViewQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? result = await connection.QueryMultipleAsync( PROCEDURE_GET_SPECS_VIEW, commandType: CommandType.StoredProcedure );

        if ( result is null )
            return null;

        IEnumerable<SpecView> intSpecs = await result.ReadAsync<SpecView>();
        IEnumerable<SpecView> stringSpecs = await result.ReadAsync<SpecView>();
        IEnumerable<SpecView> boolSpecs = await result.ReadAsync<SpecView>();
        IEnumerable<SpecView> multiSpecs = await result.ReadAsync<SpecView>();

        return new SpecsViewDto
        {
            IntSpecs = intSpecs.ToList(),
            StringSpecs = stringSpecs.ToList(),
            BoolSpecs = boolSpecs.ToList(),
            MultiSpecs = multiSpecs.ToList()
        };
    }
    static async Task<SpecLookupEditDto?> GetEditIntQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? result = await connection.QueryMultipleAsync( dynamicSql, dynamicParams, commandType: CommandType.StoredProcedure );

        if ( result is null )
            return null;

        var specView = await result.ReadFirstOrDefaultAsync<SpecView>();
        bool isGlobal = await result.ReadFirstOrDefaultAsync<bool>();
        IEnumerable<AdminSpecCategoryModel> categories = await result.ReadAsync<AdminSpecCategoryModel>();
        IEnumerable<AdminSpecFilterValueModel> values = await result.ReadAsync<AdminSpecFilterValueModel>();

        return new SpecLookupEditDto
        {
            SpecId = specView.SpecId,
            SpecName = specView.SpecName,
            SpecType = SpecLookupType.INT,
            IsGlobal = isGlobal,
            PrimaryCategoriesAsString = ConvertCategoriesToString( categories ),
            ValuesByIdAsString = ConvertIntFiltersToString( values )
        };
    }
    static async Task<SpecLookupEditDto?> GetEditStringQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? result = await connection.QueryMultipleAsync( dynamicSql, dynamicParams, commandType: CommandType.StoredProcedure );

        if ( result is null )
            return null;

        var specView = await result.ReadFirstOrDefaultAsync<SpecView>();
        bool isGlobal = await result.ReadFirstOrDefaultAsync<bool>();
        IEnumerable<AdminSpecCategoryModel> categories = await result.ReadAsync<AdminSpecCategoryModel>();
        IEnumerable<AdminSpecValueModel> values = await result.ReadAsync<AdminSpecValueModel>();

        return new SpecLookupEditDto
        {
            SpecId = specView.SpecId,
            SpecName = specView.SpecName,
            SpecType = SpecLookupType.INT,
            IsGlobal = isGlobal,
            PrimaryCategoriesAsString = ConvertCategoriesToString( categories ),
            ValuesByIdAsString = ConvertStringValuesToString( values )
        };
    }
    static async Task<SpecLookupEditDto?> GetEditBoolQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? result = await connection.QueryMultipleAsync( dynamicSql, dynamicParams, commandType: CommandType.StoredProcedure );

        if ( result is null )
            return null;

        var specView = await result.ReadFirstOrDefaultAsync<SpecView>();
        bool isGlobal = await result.ReadFirstOrDefaultAsync<bool>();
        IEnumerable<AdminSpecCategoryModel> categories = await result.ReadAsync<AdminSpecCategoryModel>();

        return new SpecLookupEditDto
        {
            SpecId = specView.SpecId,
            SpecName = specView.SpecName,
            SpecType = SpecLookupType.INT,
            IsGlobal = isGlobal,
            PrimaryCategoriesAsString = ConvertCategoriesToString( categories ),
            ValuesByIdAsString = string.Empty
        };
    }
    static async Task<int> InsertQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleOrDefaultAsync( dynamicSql, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
    }
    static async Task<bool> UpdateQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int rowsAffected = await connection.ExecuteAsync( dynamicSql, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
    }
    static async Task<bool> DeleteQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int rowsAffected = await connection.ExecuteAsync( dynamicSql, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
    }
    
    static DynamicParameters GetInsertParams( SpecLookupEditDto dto )
    {
        var parameters = new DynamicParameters();

        DataTable categoriesTable = GetPrimaryCategoriesTable( dto.PrimaryCategoriesAsString );
        
        parameters.Add( PARAM_SPEC_NAME, dto.SpecName );
        parameters.Add( PARAM_IS_GLOBAL, dto.IsGlobal );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, categoriesTable.AsTableValuedParameter( TVP_PRIMARY_CATEGORIES ) );

        if ( dto.SpecType is SpecLookupType.BOOL )
            return parameters;

        AppendInsertTableParam( dto, parameters );
        
        return parameters;
    }
    static DynamicParameters GetUpdateParams( SpecLookupEditDto dto )
    {
        DynamicParameters parameters = GetInsertParams( dto );
        parameters.Add( PARAM_SPEC_ID, dto.SpecId );
        
        return parameters;
    }
    static void AppendInsertTableParam( SpecLookupEditDto dto, DynamicParameters parameters )
    {
        DataTable valuesTable = GetSpecValuesTable( dto.SpecType, dto.ValuesByIdAsString );

        string paramNameValues = dto.SpecType switch
        {
            SpecLookupType.INT => PARAM_FILTER_VALUES,
            _ => PARAM_SPEC_VALUES
        };
        string paramTableName = dto.SpecType switch
        {
            SpecLookupType.INT => TVP_FILTER_VALUES,
            _ => TVP_SPEC_VALUES
        };

        parameters.Add( paramNameValues, valuesTable.AsTableValuedParameter( paramTableName ) );
    }
    static DataTable GetSpecValuesTable( SpecLookupType type, string valuesString )
    {
        return type switch
        {
            SpecLookupType.INT => GetStringValuesTable( valuesString, TVP_COL_FILTER_ID, TVP_COL_FILTER_VALUE ),
            SpecLookupType.STRING => GetStringValuesTable( valuesString, TVP_COL_SPEC_ID, TVP_COL_SPEC_VALUE ),
            SpecLookupType.MULTI => GetStringValuesTable( valuesString, TVP_COL_SPEC_ID, TVP_COL_SPEC_VALUE ),
            SpecLookupType.BOOL => throw new ServiceException( "Invalid SpecLookupType!", null ),
            _ => throw new ServiceException( "Invalid SpecLookupType!", null )
        };
    }
    
    static string ConvertCategoriesToString( IEnumerable<AdminSpecCategoryModel>? categories )
    {
        if ( categories is null )
            return string.Empty;
        
        var categoryIds = new List<int>();
        
        foreach ( AdminSpecCategoryModel category in categories )
        {
            categoryIds.Add( category.PrimaryCategoryId );
        }

        return string.Join( ",", categoryIds );
    }
    static string ConvertStringValuesToString( IEnumerable<AdminSpecValueModel>? values )
    {
        if ( values is null )
            return string.Empty;

        IEnumerable<string> sortedSpecValues = values
            .OrderBy( x => x.SpecValueId )
            .Select( x => x.SpecValue ?? string.Empty ); 
        
        return string.Join( ",", sortedSpecValues );
    }
    static string ConvertIntFiltersToString( IEnumerable<AdminSpecFilterValueModel>? values )
    {
        if ( values is null )
            return string.Empty;

        IEnumerable<string> sortedSpecValues = values
            .OrderBy( x => x.FilterValueId )
            .Select( x => x.FilterValue ?? string.Empty );

        return string.Join( ",", sortedSpecValues );
    }
    
    static string GetProcedure( SpecLookupType type, IReadOnlyList<string> procedures )
    {
        return procedures[ ( int ) type ];
    }
}