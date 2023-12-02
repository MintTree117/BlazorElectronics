using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Services.Users.Admin;

public class AdminFeaturedProductsServiceClient : AdminServiceClient, IAdminFeaturedProductsServiceClient
{
    const string API_ROUTE = "api/adminfeatures";
    const string API_ROUTE_GET_VIEW = API_ROUTE + "/get-features-view";
    const string API_ROUTE_GET_PRODUCT_EDIT = API_ROUTE + "/get-featured-product-edit";
    const string API_ROUTE_ADD_DEAL = API_ROUTE + "/add-featured-deal";
    const string API_ROUTE_ADD_PRODUCT = API_ROUTE + "/add-featured-product";
    const string API_ROUTE_UPDATE_PRODUCT = API_ROUTE + "/update-featured-product";
    const string API_ROUTE_REMOVE_DEAL = API_ROUTE + "/remove-featured-deal";
    const string API_ROUTE_REMOVE_PRODUCT = API_ROUTE + "/remove-featured-product";
    
    public AdminFeaturedProductsServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<List<AdminItemViewDto>?>> GetView()
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceReply<bool>> RemoveItem( IntDto itemId )
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceReply<FeaturedProductDto?>> GetEdit( IntDto dto )
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceReply<int>> Add( FeaturedProductDto dto )
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceReply<bool>> Update( FeaturedProductDto dto )
    {
        throw new NotImplementedException();
    }
}