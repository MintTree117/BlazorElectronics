using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Specs;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminSpecLookupController : _AdminController
{
    readonly ISpecsService _lookupService;

    public AdminSpecLookupController( ILogger<AdminSpecLookupController> logger, IUserAccountService userAccountService, ISessionService sessionService, ISpecsService lookupService )
        : base( logger, userAccountService, sessionService )
    {
        _lookupService = lookupService;
    }
    
    [HttpGet( "get-view" )]
    public async Task<ActionResult<List<CrudViewDto>?>> GetView()
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<CrudViewDto>?> reply = await _lookupService.GetView();
        return GetReturnFromReply( reply );
    }
    [HttpGet( "get-edit" )]
    public async Task<ActionResult<LookupSpecEditDto?>> GetEdit( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<LookupSpecEditDto?> reply = await _lookupService.GetEdit( itemId );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "add" )]
    public async Task<ActionResult<int>> Add( [FromBody] LookupSpecEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _lookupService.Add( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "update" )]
    public async Task<ActionResult<bool>> Update( [FromBody] LookupSpecEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _lookupService.Update( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpDelete( "remove" )]
    public async Task<ActionResult<bool>> Remove( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _lookupService.Remove( itemId );
        return GetReturnFromReply( reply );
    }
}