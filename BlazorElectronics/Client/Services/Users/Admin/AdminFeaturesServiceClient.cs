using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Features;

namespace BlazorElectronics.Client.Services.Users.Admin;

public class AdminFeaturesServiceClient : AdminServiceClient, IAdminFeaturesServiceClient
{
    const string API_ROUTE = "api/adminfeatures";
    const string API_ROUTE_GET_VIEW = API_ROUTE + "/get-features-view";
    const string API_ROUTE_GET_PRODUCT_EDIT = API_ROUTE + "/get-featured-product-edit";
    const string API_ROUTE_ADD_DEAL = API_ROUTE + "/add-featured-deal";
    const string API_ROUTE_ADD_PRODUCT = API_ROUTE + "/add-featured-product";
    const string API_ROUTE_UPDATE_PRODUCT = API_ROUTE + "/update-featured-product";
    const string API_ROUTE_REMOVE_DEAL = API_ROUTE + "/remove-featured-deal";
    const string API_ROUTE_REMOVE_PRODUCT = API_ROUTE + "/remove-featured-product";
    
    public AdminFeaturesServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ApiReply<FeaturesViewDto?>> GetFeaturesView()
    {
        return await TryUserRequest<FeaturesViewDto?>( API_ROUTE_GET_VIEW );
    }
    public async Task<ApiReply<FeaturedProductEditDto?>> GetFeaturedProductEdit( IntDto dto )
    {
        return await TryUserRequest<IntDto, FeaturedProductEditDto?>( API_ROUTE_GET_PRODUCT_EDIT, dto );
    }
    public async Task<ApiReply<bool>> AddFeaturedProduct( FeaturedProductEditDto dto )
    {
        return await TryUserRequest<FeaturedProductEditDto, bool>( API_ROUTE_ADD_PRODUCT, dto );
    }
    public async Task<ApiReply<bool>> AddFeaturedDeal( IntDto dto )
    {
        return await TryUserRequest<IntDto, bool>( API_ROUTE_ADD_DEAL, dto );
    }
    public async Task<ApiReply<bool>> UpdateFeaturedProduct( FeaturedProductEditDto dto )
    {
        return await TryUserRequest<FeaturedProductEditDto, bool>( API_ROUTE_UPDATE_PRODUCT, dto );
    }
    public async Task<ApiReply<bool>> RemoveFeaturedProduct( IntDto dto )
    {
        return await TryUserRequest<IntDto, bool>( API_ROUTE_REMOVE_PRODUCT, dto );
    }
    public async Task<ApiReply<bool>> RemoveFeaturedDeal( IntDto dto )
    {
        return await TryUserRequest<IntDto, bool>( API_ROUTE_REMOVE_DEAL, dto );
    }
}