using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Features;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminFeaturesController : _AdminController
{
    readonly IFeaturesService _featuresService;
    
    public AdminFeaturesController( ILogger<_UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, IFeaturesService featuresService )
        : base( logger, userAccountService, sessionService )
    {
        _featuresService = featuresService;
    }
    
    [HttpGet( "get-view" )]
    public async Task<ActionResult<List<CrudViewDto>?>> GetView()
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<CrudViewDto>?> reply = await _featuresService.GetFeaturesView();
        return GetReturnFromReply( reply );
    }
    [HttpGet( "get-edit" )]
    public async Task<ActionResult<FeatureDtoEditDto?>> GetEdit( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<FeatureDtoEditDto?> reply = await _featuresService.GetFeatureEdit( itemId );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<int>> Add( [FromBody] FeatureDtoEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _featuresService.AddFeature( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "update" )]
    public async Task<ActionResult<bool>> Update( [FromBody] FeatureDtoEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _featuresService.UpdateFeature( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpDelete( "remove" )]
    public async Task<ActionResult<bool>> Remove( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _featuresService.RemoveFeature( itemId );
        return GetReturnFromReply( reply );
    }
}