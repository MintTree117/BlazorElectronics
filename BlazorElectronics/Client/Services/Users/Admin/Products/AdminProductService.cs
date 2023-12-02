using Blazored.LocalStorage;

namespace BlazorElectronics.Client.Services.Users.Admin.Products;

public sealed class AdminProductService : AdminServiceClient, IAdminProductService
{
    public AdminProductService( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
}