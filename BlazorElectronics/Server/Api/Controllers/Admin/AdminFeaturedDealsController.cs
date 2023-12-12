using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Features;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminFeaturedDealsController : _AdminController
{
    readonly IFeaturesService _featuresService;
    
    public AdminFeaturedDealsController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, IFeaturesService featuresService )
        : base( logger, userAccountService, sessionService )
    {
        _featuresService = featuresService;
    }

    [HttpPost( "get-view" )]
    public async Task<ActionResult<List<CrudView>>> GetView( [FromBody] UserRequest request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<CrudView>?> reply = await _featuresService.GetDealsView();
        return GetReturnFromReply( reply );
    }
    [HttpPost( "get-edit" )]
    public async Task<ActionResult<FeaturedDealEdit?>> GetEdit( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<FeaturedDealEdit?> reply = await _featuresService.GetDealEdit( request.Payload.Value );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<int>> Add( [FromBody] UserDataRequest<FeaturedDealEdit> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _featuresService.AddDeal( request.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "update" )]
    public async Task<ActionResult<bool>> Update( [FromBody] UserDataRequest<FeaturedDealEdit> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _featuresService.UpdateDeal( request.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<bool>> Remove( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _featuresService.RemoveDeal( request.Payload.Value );
        return GetReturnFromReply( reply );
    }
}