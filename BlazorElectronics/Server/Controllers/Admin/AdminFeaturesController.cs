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
    
    [HttpPost( "get-view" )]
    public async Task<ActionResult<List<CrudView>?>> GetView( [FromBody] UserRequest request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<List<CrudView>?> reply = await _featuresService.GetFeaturesView();
        return GetReturnFromApi( reply );
    }
    [HttpPost( "get-edit" )]
    public async Task<ActionResult<FeatureEdit?>> GetEdit( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<FeatureEdit?> reply = await _featuresService.GetFeatureEdit( request.Payload.Value );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<int>> Add( [FromBody] UserDataRequest<FeatureEdit> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<int> reply = await _featuresService.AddFeature( request.Payload );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "update" )]
    public async Task<ActionResult<bool>> Update( [FromBody] UserDataRequest<FeatureEdit> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<bool> reply = await _featuresService.UpdateFeature( request.Payload );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<bool>> Remove( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<bool> reply = await _featuresService.RemoveFeature( request.Payload.Value );
        return GetReturnFromApi( reply );
    }
}