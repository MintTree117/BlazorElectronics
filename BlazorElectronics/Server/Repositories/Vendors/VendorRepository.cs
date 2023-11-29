using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Vendors;
using BlazorElectronics.Shared.Admin.Vendors;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Vendors;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Vendors;

public class VendorRepository : DapperRepository, IVendorRepository
{
    const string PROCEDURE_GET_VENDORS = "Get_Vendors";
    const string PROCEDURE_GET_VIEW = "Get_VendorsView";
    const string PROCEDURE_GET_EDIT = "Get_VendorEdit";
    const string PROCEDURE_INSERT = "Insert_Vendor";
    const string PROCEDURE_UPDATE = "Update_Vendor";
    const string PROCEDURE_DELETE = "Delete_Vendor";
    
    public VendorRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<VendorsModel?> Get()
    { 
        return await TryQueryAsync( GetQuery );
    }
    public Task<VendorEditModel?> GetEdit( int vendorId )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_VENDOR_ID, vendorId );

        return TryQueryAsync( GetEditQuery, parameters );
    }
    public Task<int> Insert( VendorEditDto dto )
    {
        DynamicParameters parameters = GetInsertParameters( dto );

        return TryQueryTransactionAsync( InsertQuery, parameters );
    }
    public Task<bool> Update( VendorEditDto dto )
    {
        DynamicParameters parameters = GetUpdateParameters( dto );

        return TryQueryTransactionAsync( UpdateQuery, parameters );
    }
    public Task<bool> Delete( int vendorId )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_VENDOR_ID, vendorId );

        return TryQueryTransactionAsync( RemoveQuery, parameters );
    }

    static async Task<VendorsModel?> GetQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET_VENDORS, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        IEnumerable<VendorModel>? vendors = await multi.ReadAsync<VendorModel>();
        IEnumerable<VendorCategoryModel>? categories = await multi.ReadAsync<VendorCategoryModel>();

        return new VendorsModel
        {
            Vendors = vendors,
            Categories = categories
        };
    }
    static async Task<VendorEditModel?> GetEditQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET_EDIT, dynamicParams, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;
        
        return new VendorEditModel
        {
            Vendor = await multi.ReadSingleOrDefaultAsync<VendorModel>(),
            Categories = await multi.ReadAsync<VendorCategoryModel>()
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
    static async Task<bool> RemoveQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int rowsAffected = await connection.ExecuteAsync( PROCEDURE_DELETE, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
    }
    
    static DynamicParameters GetInsertParameters( VendorEditDto dto )
    {
        var parameters = new DynamicParameters();

        parameters.Add( PARAM_VENDOR_NAME, dto.VendorName );
        parameters.Add( PARAM_VENDOR_URL, dto.VendorUrl );

        DataTable categoriesTable = GetPrimaryCategoriesTable( dto.PrimaryCategories );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, categoriesTable.AsTableValuedParameter( TVP_PRIMARY_CATEGORIES ) );
        
        return parameters;
    }
    static DynamicParameters GetUpdateParameters( VendorEditDto dto )
    {
        DynamicParameters parameters = GetInsertParameters( dto );
        parameters.Add( PARAM_VENDOR_ID, dto.VendorId );
        
        return parameters;
    }
}