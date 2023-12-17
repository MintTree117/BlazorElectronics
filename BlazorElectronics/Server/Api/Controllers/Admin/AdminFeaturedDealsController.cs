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
    public async Task<ActionResult<List<CrudViewDto>>> GetView( [FromBody] UserRequestDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<CrudViewDto>?> reply = await _featuresService.GetDealsView();
        return GetReturnFromReply( reply );
    }
    [HttpPost( "get-edit" )]
    public async Task<ActionResult<FeaturedDealDtoEditDto?>> GetEdit( [FromBody] UserDataRequestDto<IntDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<FeaturedDealDtoEditDto?> reply = await _featuresService.GetDealEdit( requestDto.Payload.Value );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<int>> Add( [FromBody] UserDataRequestDto<FeaturedDealDtoEditDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _featuresService.AddDeal( requestDto.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "update" )]
    public async Task<ActionResult<bool>> Update( [FromBody] UserDataRequestDto<FeaturedDealDtoEditDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _featuresService.UpdateDeal( requestDto.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<bool>> Remove( [FromBody] UserDataRequestDto<IntDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _featuresService.RemoveDeal( requestDto.Payload.Value );
        return GetReturnFromReply( reply );
    }
}