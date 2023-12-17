using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Products;

namespace BlazorElectronics.Client.Services.Admin;

public interface IAdminBulkServiceClient
{
    Task<ServiceReply<bool>> BulkInsertCategories( List<CategoryEditDtoDto> categories );
    Task<ServiceReply<bool>> BulkInsertKeys( ProductKeysDto keysDto );
}