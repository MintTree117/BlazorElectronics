using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Products;

namespace BlazorElectronics.Client.Services.Admin;

public interface IAdminBulkServiceClient
{
    Task<ServiceReply<bool>> BulkInsertCategories( List<CategoryEdit> categories );
    Task<ServiceReply<bool>> BulkInsertKeys( ProductKeys keys );
}