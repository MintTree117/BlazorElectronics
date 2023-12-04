using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Vendors;
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
    public async Task<IEnumerable<VendorModel>?> GetView()
    {
        return await TryQueryAsync( Query<VendorModel>, null, PROCEDURE_GET_VIEW );
    }
    public async Task<VendorEditModel?> GetEdit( int vendorId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_VENDOR_ID, vendorId );
        return await TryQueryAsync( GetEditQuery, p );
    }
    public async Task<int> Insert( VendorEdit dto )
    {
        DynamicParameters p = GetInsertParameters( dto );
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_INSERT );
    }
    public async Task<bool> Update( VendorEdit dto )
    {
        DynamicParameters p = GetUpdateParameters( dto );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE );
    }
    public async Task<bool> Delete( int vendorId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_VENDOR_ID, vendorId );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE );
    }

    static async Task<VendorsModel?> GetQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET_VENDORS, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        return new VendorsModel
        {
            Vendors = await multi.ReadAsync<VendorModel>(),
            Categories = await multi.ReadAsync<VendorCategoryModel>()
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
    
    static DynamicParameters GetInsertParameters( VendorEdit dto )
    {
        var parameters = new DynamicParameters();

        parameters.Add( PARAM_VENDOR_NAME, dto.VendorName );
        parameters.Add( PARAM_VENDOR_URL, dto.VendorUrl );

        DataTable categoriesTable = GetPrimaryCategoriesTable( dto.PrimaryCategories );
        parameters.Add( PARAM_PRIMARY_CATEGORIES, categoriesTable.AsTableValuedParameter( TVP_PRIMARY_CATEGORIES ) );
        
        return parameters;
    }
    static DynamicParameters GetUpdateParameters( VendorEdit dto )
    {
        DynamicParameters parameters = GetInsertParameters( dto );
        parameters.Add( PARAM_VENDOR_ID, dto.VendorId );
        
        return parameters;
    }
}