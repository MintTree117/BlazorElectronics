using System.Data;
using BlazorElectronics.Server.Admin.Models;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Admin.Specs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Admin.Repositories;

public class AdminSpecLookupRepository : _AdminRepository, IAdminSpecLookupRepository
{
    static readonly string[] PROCEDURES_GET_EDIT =
    {
        "Get_SpecLookupSingleIntEdit",
        "Get_SpecLookupSingleStringEdit",
        "Get_SpecLookupSingleBoolEdit",
        "Get_SpecLookupMultiStringEdit"
    };
    static readonly string[] PROCEDURES_INSERT =
    {
        "Insert_SpecLookupSingleInt",
        "Insert_SpecLookupSingleString",
        "Insert_SpecLookupSingleBool",
        "Insert_SpecLookupMultiString"
    };
    static readonly string[] PROCEDURES_UPDATE =
    {
        "Update_SpecLookupSingleInt", 
        "Update_SpecLookupSingleString", 
        "Update_SpecLookupSingleBool",
        "Update_SpecLookupMultiString"
    };
    static readonly string[] PROCEDURES_DELETE =
    {
        "Delete_SpecLookupSingleInt", 
        "Delete_SpecLookupSingleString",
        "Delete_SpecLookupSingleBool",
        "Delete_SpecLookupMultiString"
    };
    
    const string PROCEDURE_GET_SPECS_VIEW = "Get_SpecLookupView";

    public AdminSpecLookupRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public async Task<SpecsViewDto?> GetSpecsView()
    {
        return await TryQueryAsync( GetSpecsViewQuery );
    }
    public async Task<EditSpecLookupDto?> GetSpecEdit( GetSpecLookupEditDto dto )
    {
        string procedure = GetProcedure( dto.SpecType, PROCEDURES_GET_EDIT );
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_ID, dto.SpedId );

        DapperQueryDelegate<EditSpecLookupDto> method = dto.SpecType switch
        {
            SpecLookupType.INT => GetIntSpecEditQuery,
            SpecLookupType.BOOL => GetBoolSpecEditQuery,
            SpecLookupType.STRING => GetStringSpecEditQuery,
            SpecLookupType.MULTI => GetStringSpecEditQuery,
            _ => throw new Exception( "Invalid SpecLookupType!" )
        };

        EditSpecLookupDto? edit = await TryQueryAsync( method, parameters, procedure );

        if ( edit is null )
            return null;
        
        edit.SpecType = dto.SpecType;
        return edit;
    }
    public async Task<int> Insert( EditSpecLookupDto dto )
    {
        string procedure = GetProcedure( dto.SpecType, PROCEDURES_INSERT );
        DynamicParameters parameters = GetAddParams( dto );

        return await TryAdminQueryTransaction<int>( procedure, parameters );
    }
    public async Task<bool> Update( EditSpecLookupDto dto )
    {
        string procedure = GetProcedure( dto.SpecType, PROCEDURES_UPDATE );
        DynamicParameters parameters = GetUpdateParams( dto );

        return await TryAdminTransaction( procedure, parameters );
    }
    public async Task<bool> Delete( RemoveSpecLookupDto dto )
    {
        string procedure = GetProcedure( dto.SpecType, PROCEDURES_DELETE );
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SPEC_ID, dto.SpecId );

        return await TryAdminTransaction( procedure, parameters );
    }
    
    static async Task<SpecsViewDto?> GetSpecsViewQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
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
    static async Task<EditSpecLookupDto?> GetIntSpecEditQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? result = await connection.QueryMultipleAsync( dynamicSql, dynamicParams, commandType: CommandType.StoredProcedure );

        if ( result is null )
            return null;

        var specView = await result.ReadFirstOrDefaultAsync<SpecView>();
        bool isGlobal = await result.ReadFirstOrDefaultAsync<bool>();
        IEnumerable<int> categories = await result.ReadAsync<int>();
        IEnumerable<AdminSpecFilterValueModel> values = await result.ReadAsync<AdminSpecFilterValueModel>();

        return new EditSpecLookupDto
        {
            SpecId = specView.SpecId,
            SpecName = specView.SpecName,
            SpecType = SpecLookupType.INT,
            IsGlobal = isGlobal,
            PrimaryCategoriesAsString = ConvertCategoriesToString( categories ),
            ValuesByIdAsString = ConvertIntFiltersToString( values )
        };
    }
    static async Task<EditSpecLookupDto?> GetStringSpecEditQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? result = await connection.QueryMultipleAsync( dynamicSql, dynamicParams, commandType: CommandType.StoredProcedure );

        if ( result is null )
            return null;

        var specView = await result.ReadFirstOrDefaultAsync<SpecView>();
        bool isGlobal = await result.ReadFirstOrDefaultAsync<bool>();
        IEnumerable<int> categories = await result.ReadAsync<int>();
        IEnumerable<AdminSpecValueModel> values = await result.ReadAsync<AdminSpecValueModel>();

        return new EditSpecLookupDto
        {
            SpecId = specView.SpecId,
            SpecName = specView.SpecName,
            SpecType = SpecLookupType.INT,
            IsGlobal = isGlobal,
            PrimaryCategoriesAsString = ConvertCategoriesToString( categories ),
            ValuesByIdAsString = ConvertStringValuesToString( values )
        };
    }
    static async Task<EditSpecLookupDto?> GetBoolSpecEditQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? result = await connection.QueryMultipleAsync( dynamicSql, dynamicParams, commandType: CommandType.StoredProcedure );

        if ( result is null )
            return null;

        var specView = await result.ReadFirstOrDefaultAsync<SpecView>();
        bool isGlobal = await result.ReadFirstOrDefaultAsync<bool>();
        IEnumerable<int> categories = await result.ReadAsync<int>();

        return new EditSpecLookupDto
        {
            SpecId = specView.SpecId,
            SpecName = specView.SpecName,
            SpecType = SpecLookupType.INT,
            IsGlobal = isGlobal,
            PrimaryCategoriesAsString = ConvertCategoriesToString( categories ),
            ValuesByIdAsString = string.Empty
        };
    }
    
    static DynamicParameters GetAddParams( EditSpecLookupDto dto )
    {
        var parameters = new DynamicParameters();

        DataTable categoriesTable = GetPrimaryCategoriesTable( dto.PrimaryCategoriesAsString );
        DataTable? valuesTable = dto.SpecType is not SpecLookupType.BOOL
            ? GetSpecValuesTable( dto.SpecType, dto.ValuesByIdAsString )
            : null;

        parameters.Add( PARAM_SPEC_NAME, dto.SpecName );
        parameters.Add( PARAM_IS_GLOBAL, dto.IsGlobal );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, categoriesTable.AsTableValuedParameter( PARAM_TVP_PRIMARY_CATEGORIES ) );

        if ( dto.SpecType is SpecLookupType.BOOL )
            return parameters;
        
        string paramNameValues = dto.SpecType switch
        {
            SpecLookupType.INT => PARAM_FILTER_VALUES,
            _ => PARAM_SPEC_VALUES
        };
        
        string paramTableName = dto.SpecType switch
        {
            SpecLookupType.INT => PARAM_TVP_FILTER_VALUES,
            _ => PARAM_TVP_SPEC_VALUES
        };
        
        parameters.Add( paramNameValues, valuesTable.AsTableValuedParameter( paramTableName ) );
        
        return parameters;
    }
    static DynamicParameters GetUpdateParams( EditSpecLookupDto dto )
    {
        DynamicParameters parameters = GetAddParams( dto );
        parameters.Add( PARAM_SPEC_ID, dto.SpecId );
        
        return parameters;
    }

    static string ConvertCategoriesToString( IEnumerable<int>? categories )
    {
        return categories is null ? string.Empty : string.Join( ",", categories );
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
    
    static DataTable GetSpecValuesTable( SpecLookupType type, string valuesString )
    {
        List<string> values = valuesString
            .Split( ',' )
            .Select( s => s.Trim() ) // Trims whitespace from each item.
            .ToList();
        
        var table = new DataTable();

        string idCol;
        string valueCol;
        
        switch ( type )
        {
            case SpecLookupType.INT:
                idCol = TVP_COL_FILTER_ID;
                valueCol = TVP_COL_FILTER_VALUE;
                break;
            case SpecLookupType.STRING:
            case SpecLookupType.MULTI:
                idCol = TVP_COL_SPEC_ID;
                valueCol = TVP_COL_SPEC_VALUE;
                break;
            case SpecLookupType.BOOL:
            default:
                throw new ServiceException( "Invalid SpecLookupType!", null );
        }

        table.Columns.Add( idCol, typeof( int ) );
        table.Columns.Add( valueCol, typeof( string ) );

        for ( int i = 0; i < values.Count; i++ )
        {
            DataRow row = table.NewRow();
            row[ idCol ] = i + 1;
            row[ valueCol ] = values[ i ];
            table.Rows.Add( row );
        }

        return table;
    }
    static string GetProcedure( SpecLookupType type, IReadOnlyList<string> procedures )
    {
        return procedures[ ( int ) type ];
    }
}