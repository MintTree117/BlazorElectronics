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
    public async Task<ActionResult<List<AdminItemViewDto>>> GetFeauresView( [FromBody] UserRequest request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<List<AdminItemViewDto>?> reply = await _featuresService.GetFeaturesView();
        return GetReturnFromApi( reply );
    }
    [HttpPost( "get-deals-view" )]
    public async Task<ActionResult<List<AdminItemViewDto>>> GetDealsView( [FromBody] UserRequest request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<List<AdminItemViewDto>?> reply = await _featuresService.GetDealsView();
        return GetReturnFromApi( reply );
    }
    [HttpPost( "add-feature" )]
    public async Task<ActionResult<Feature>> AddFeature( [FromBody] UserDataRequest<Feature> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<Feature?> reply = await _featuresService.AddFeature( request.Payload );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "add-featured-deal" )]
    public async Task<ActionResult<FeaturedDeal>> AddDeal( [FromBody] UserDataRequest<FeaturedDeal> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<FeaturedDeal?> reply = await _featuresService.AddDeal( request.Payload );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "update-feature" )]
    public async Task<ActionResult<bool>> UpdateFeature( [FromBody] UserDataRequest<Feature> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<bool> reply = await _featuresService.UpdateFeature( request.Payload );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "update-featured-deal" )]
    public async Task<ActionResult<bool>> UpdateFeature( [FromBody] UserDataRequest<FeaturedDeal> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<bool> reply = await _featuresService.UpdateDeal( request.Payload );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "remove-feature" )]
    public async Task<ActionResult<bool>> RemoveFeature( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<bool> reply = await _featuresService.RemoveFeature( request.Payload.Value );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "remove-featured-deal" )]
    public async Task<ActionResult<bool>> RemoveDeal( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<bool> reply = await _featuresService.RemoveDeal( request.Payload.Value );
        return GetReturnFromApi( reply );
    }
}