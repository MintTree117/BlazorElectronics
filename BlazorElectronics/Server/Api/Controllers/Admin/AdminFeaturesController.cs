using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Features;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

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
    public async Task<ActionResult<List<CrudViewDto>?>> GetView( [FromBody] UserRequestDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<CrudViewDto>?> reply = await _featuresService.GetFeaturesView();
        return GetReturnFromReply( reply );
    }
    [HttpPost( "get-edit" )]
    public async Task<ActionResult<FeatureDtoEditDto?>> GetEdit( [FromBody] UserDataRequestDto<IntDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<FeatureDtoEditDto?> reply = await _featuresService.GetFeatureEdit( requestDto.Payload.Value );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<int>> Add( [FromBody] UserDataRequestDto<FeatureDtoEditDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _featuresService.AddFeature( requestDto.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "update" )]
    public async Task<ActionResult<bool>> Update( [FromBody] UserDataRequestDto<FeatureDtoEditDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _featuresService.UpdateFeature( requestDto.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<bool>> Remove( [FromBody] UserDataRequestDto<IntDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _featuresService.RemoveFeature( requestDto.Payload.Value );
        return GetReturnFromReply( reply );
    }
}