using System.Data;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Vendors;
using BlazorElectronics.Shared.Vendors;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Data.Repositories;

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
    public async Task<IEnumerable<VendorDto>?> GetView()
    {
        return await TryQueryAsync( Query<VendorDto>, null, PROCEDURE_GET_VIEW );
    }
    public async Task<VendorEditModel?> GetEdit( int vendorId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_VENDOR_ID, vendorId );
        return await TryQueryAsync( GetEditQuery, p );
    }
    public async Task<int> Insert( VendorEditDtoDto dtoDto )
    {
        DynamicParameters p = GetInsertParameters( dtoDto );
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_INSERT );
    }
    public async Task<bool> Update( VendorEditDtoDto dtoDto )
    {
        DynamicParameters p = GetUpdateParameters( dtoDto );
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
            Vendors = await multi.ReadAsync<VendorDto>(),
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
            Vendor = await multi.ReadSingleOrDefaultAsync<VendorDto>(),
            Categories = await multi.ReadAsync<VendorCategoryModel>()
        };
    }
    
    static DynamicParameters GetInsertParameters( VendorEditDtoDto dtoDto )
    {
        var parameters = new DynamicParameters();

        parameters.Add( PARAM_VENDOR_NAME, dtoDto.VendorName );
        parameters.Add( PARAM_VENDOR_URL, dtoDto.VendorUrl );
        parameters.Add( PARAM_IS_GLOBAL, dtoDto.IsGlobal );

        DataTable categoriesTable = GetPrimaryCategoriesTable( dtoDto.PrimaryCategories );
        parameters.Add( PARAM_CATEGORY_IDS, categoriesTable.AsTableValuedParameter( TVP_CATEGORY_IDS ) );
        
        return parameters;
    }
    static DynamicParameters GetUpdateParameters( VendorEditDtoDto dtoDto )
    {
        DynamicParameters parameters = GetInsertParameters( dtoDto );
        parameters.Add( PARAM_VENDOR_ID, dtoDto.VendorId );
        
        return parameters;
    }
}