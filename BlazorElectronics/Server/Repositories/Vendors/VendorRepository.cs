using System.Data;
using BlazorElectronics.Server.Admin.Models.Vendors;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.SpecLookups;
using BlazorElectronics.Shared.Vendors;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Vendors;

public sealed class VendorRepository : DapperRepository, IVendorRepository
{
    const string PROCEDURE_GET_VENDORS = "Get_Vendors";
    
    public VendorRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public Task<VendorsResponse?> GetVendors()
    {
        throw new NotImplementedException();
    }

    static async Task<VendorsResponse?> GetSpecLookupData( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET_VENDORS, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        var vendors = await multi.ReadAsync<VendorModel>();
        var categories = await multi.ReadAsync<VendorCategoryModel>();

        return null;
    }
}