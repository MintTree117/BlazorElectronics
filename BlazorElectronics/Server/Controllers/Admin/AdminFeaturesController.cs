using BlazorElectronics.Server.Repositories.Features;
using BlazorElectronics.Server.Services.Features;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Features;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminFeaturesController : _AdminController
{
    readonly IFeaturesService _featuresService;
    
    public AdminFeaturesController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, IFeaturesService featuresService )
        : base( logger, userAccountService, sessionService )
    {
        _featuresService = featuresService;
    }
    
    [HttpPost( "get-features-view" )]
    public async Task<ActionResult<FeaturesResponse?>> GetView( [FromBody] UserRequest? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<FeaturesResponse?> reply = await _featuresService.GetView();
        return GetReturnFromApi( reply );
    }
    [HttpPost( "get-featured-product-edit" )]
    public async Task<ActionResult<FeaturedProductDto?>> GetProductEdit( [FromBody] UserDataRequest<IntDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<FeaturedProductDto?> reply = await _featuresService.GetFeaturedProductEdit( request!.Payload!.Value );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "add-featured-product" )]
    public async Task<ActionResult<bool>> AddProduct( [FromBody] UserDataRequest<FeaturedProductDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<bool> reply = await _featuresService.AddFeaturedProduct( request!.Payload! );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "add-featured-deal" )]
    public async Task<ActionResult<bool>> AddDeal( [FromBody] UserDataRequest<IntDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<bool> reply = await _featuresService.AddFeaturedDeal( request!.Payload!.Value );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "update-featured-product" )]
    public async Task<ActionResult<bool>> UpdateProduct( [FromBody] UserDataRequest<FeaturedProductDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<bool> reply = await _featuresService.UpdateFeaturedProduct( request!.Payload! );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "remove-featured-product" )]
    public async Task<ActionResult<bool>> RemoveProduct( [FromBody] UserDataRequest<IntDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<bool> reply = await _featuresService.RemoveFeaturedProduct( request!.Payload!.Value );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "remove-featured-deal" )]
    public async Task<ActionResult<bool>> RemoveDeal( [FromBody] UserDataRequest<IntDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<bool> reply = await _featuresService.RemoveFeaturedDeal( request!.Payload!.Value );
        return GetReturnFromApi( reply );
    }
}