using System.Data;
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
    
    const string PROCEDURE_GET_SPECS_VIEW = "Get_SpecsAdminView";
    
    const string PARAM_PRIMARY_CATEGORIES = "@PrimaryCategories";
    const string PARAM_IS_GLOBAL = "@IsGlobal";
    const string PARAM_SPEC_ID = "@SpecId";
    const string PARAM_SPEC_NAME = "@SpecName";
    const string PARAM_FILTER_VALUES = "@FilterValue";
    const string PARAM_SPEC_VALUES = "@SpecValues";
    
    public AdminSpecLookupRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public async Task<SpecsViewDto?> GetSpecsView()
    {
        return await TryQueryAsync( GetSpecsViewQuery );
    }
    public async Task<EditSpecLookupDto?> GetSpecEdit( GetSpecLookupEditDto dto )
    {
        return null;
    }
    public async Task<EditSpecLookupDto?> Insert( AddSpecLookupDto dto )
    {
        string procedure = GetProcedure( dto.EditDto.SpecType, PROCEDURES_INSERT );
        DynamicParameters parameters = GetAddParams( dto );

        return await TryAdminQueryTransaction<EditSpecLookupDto?>( procedure, parameters );
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

        IEnumerable<SpecView>? intSpecs = await result.ReadAsync<SpecView>();
        IEnumerable<SpecView>? stringSpecs = await result.ReadAsync<SpecView>();
        IEnumerable<SpecView>? boolSpecs = await result.ReadAsync<SpecView>();
        IEnumerable<SpecView>? multiSpecs = await result.ReadAsync<SpecView>();

        return new SpecsViewDto
        {
            IntSpecs = intSpecs is not null ? intSpecs.ToList() : new List<SpecView>(),
            StringSpecs = intSpecs is not null ? stringSpecs.ToList() : new List<SpecView>(),
            BoolSpecs = intSpecs is not null ? boolSpecs.ToList() : new List<SpecView>(),
            MultiSpecs = intSpecs is not null ? multiSpecs.ToList() : new List<SpecView>()
        };
    }
    
    static DynamicParameters GetAddParams( AddSpecLookupDto dto )
    {
        DynamicParameters parameters = GetUpdateParams( dto.EditDto );
        parameters.Add( PARAM_SPEC_ID, dto.SpecId );
        
        return parameters;
    }
    static DynamicParameters GetUpdateParams( EditSpecLookupDto dto )
    {
        var parameters = new DynamicParameters();

        DataTable categoriesTable = GetSpecCategoriesTable( dto.PrimaryCategoriesAsString );
        DataTable valuesTable = GetSpecValuesTable( dto.SpecType, dto.ValuesByIdAsString );

        parameters.Add( PARAM_SPEC_NAME, dto.SpecName );
        parameters.Add( PARAM_IS_GLOBAL, dto.IsGlobal );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, categoriesTable );
        parameters.Add( PARAM_SPEC_VALUES, valuesTable );

        return parameters;
    }
    static DataTable GetSpecCategoriesTable( string categoriesString )
    {
        List<string> categoryStrings = categoriesString
            .Split( ',' )
            .Select( s => s.Trim() ) // Trims whitespace from each item.
            .ToList();

        var categories = new List<int>();

        foreach ( string c in categoryStrings )
        {
            if ( int.TryParse( c, out int category ) )
                categories.Add( category );
        }
        
        var table = new DataTable();
        table.Columns.Add( "PrimaryCategoryId", typeof( int ) );

        foreach ( int id in categories )
            table.Rows.Add( id );
        
        return table;
    }
    static DataTable GetSpecValuesTable( SpecLookupType type, string valuesString )
    {
        List<string> values = valuesString
            .Split( ',' )
            .Select( s => s.Trim() ) // Trims whitespace from each item.
            .ToList();
        
        var table = new DataTable();

        switch ( type )
        {
            case SpecLookupType.INT:
                table.Columns.Add( "SpecId", typeof( int ) );
                table.Columns.Add( "SpecValue", typeof( string ) );
                break;
            case SpecLookupType.STRING:
            case SpecLookupType.MULTI:
                table.Columns.Add( "FilterId", typeof( int ) );
                table.Columns.Add( "FilterValue", typeof( string ) );
                break;
            case SpecLookupType.BOOL:
            default:
                throw new ServiceException( "Invalid SpecLookupType!", null );
        }

        for ( int i = 0; i < values.Count; i++ )
            table.Rows.Add( i, values[ i ] );

        return table;
    }
    static string GetProcedure( SpecLookupType type, IReadOnlyList<string> procedures )
    {
        return procedures[ ( int ) type ];
    }
}