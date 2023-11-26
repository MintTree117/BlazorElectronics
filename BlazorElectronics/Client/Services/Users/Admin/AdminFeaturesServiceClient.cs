using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Features;

namespace BlazorElectronics.Client.Services.Users.Admin;

public class AdminFeaturesServiceClient : AdminServiceClient, IAdminFeaturesServiceClient
{
    const string API_ROUTE = "api/adminfeatures";
    const string API_ROUTE_GET_VIEW = API_ROUTE + "/get-features-view";
    const string API_ROUTE_ADD_DEAL = API_ROUTE + "/add-featured-deal";
    const string API_ROUTE_ADD_PRODUCT = API_ROUTE + "/add-featured-product";
    const string API_ROUTE_UPDATE_PRODUCT = API_ROUTE + "/update-featured-product";
    const string API_ROUTE_REMOVE_DEAL = API_ROUTE + "/remove-featured-deal";
    const string API_ROUTE_REMOVE_PRODUCT = API_ROUTE + "/remove-featured-product";
    
    FeaturesViewDto? _features;
    
    public AdminFeaturesServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ApiReply<FeaturesViewDto?>> GetFeaturesView()
    {
        ApiReply<FeaturesViewDto?> reply = await TryUserRequest<FeaturesViewDto?>( API_ROUTE_GET_VIEW );

        if ( reply is { Success: true, Data: not null } )
            _features = reply.Data;

        return reply;
    }
    public async Task<ApiReply<FeaturedProductEditDto?>> GetFeaturedProductEdit( int productId )
    {
        FeaturedProductEditDto? featuredProduct = _features?.FeaturedProducts.First( p => p.ProductId == productId );

        if ( featuredProduct != null )
            return new ApiReply<FeaturedProductEditDto?>( featuredProduct );

        ApiReply<FeaturesViewDto?> reply = await GetFeaturesView();

        if ( !reply.Success || _features is null )
            return new ApiReply<FeaturedProductEditDto?>( reply.Message );

        featuredProduct = _features?.FeaturedProducts.First( p => p.ProductId == productId );

        return featuredProduct is not null
            ? new ApiReply<FeaturedProductEditDto?>( featuredProduct )
            : new ApiReply<FeaturedProductEditDto?>( "Feature not found!" );
    }
    public async Task<ApiReply<bool>> AddFeaturedProduct( FeaturedProductEditDto dto )
    {
        return await TryUserRequest<FeaturedProductEditDto, bool>( API_ROUTE_ADD_PRODUCT, dto );
    }
    public async Task<ApiReply<bool>> AddFeaturedDeal( IdDto dto )
    {
        return await TryUserRequest<IdDto, bool>( API_ROUTE_ADD_DEAL, dto );
    }
    public async Task<ApiReply<bool>> UpdateFeaturedProduct( FeaturedProductEditDto dto )
    {
        return await TryUserRequest<FeaturedProductEditDto, bool>( API_ROUTE_UPDATE_PRODUCT, dto );
    }
    public async Task<ApiReply<bool>> RemoveFeaturedProduct( IdDto dto )
    {
        return await TryUserRequest<IdDto, bool>( API_ROUTE_REMOVE_PRODUCT, dto );
    }
    public async Task<ApiReply<bool>> RemoveFeaturedDeal( IdDto dto )
    {
        return await TryUserRequest<IdDto, bool>( API_ROUTE_REMOVE_DEAL, dto );
    }
}