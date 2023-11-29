using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.SpecLookups;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.SpecLookups;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminSpecLookupController : _AdminController
{
    readonly ISpecLookupService _lookupService;

    public AdminSpecLookupController( ILogger<AdminSpecLookupController> logger, IUserAccountService userAccountService, ISessionService sessionService, ISpecLookupService lookupService )
        : base( logger, userAccountService, sessionService )
    {
        _lookupService = lookupService;
    }
    
    [HttpPost( "get-spec-lookup-view" )]
    public async Task<ActionResult<SpecLookupViewResponse?>> GetView( [FromBody] UserRequest? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<SpecLookupViewResponse?> reply = await _lookupService.GetView();
        return GetReturnFromApi( reply );
    }
    [HttpPost( "get-spec-lookup-edit" )]
    public async Task<ActionResult<SpecLookupEditDto?>> GetEdit( [FromBody] UserDataRequest<IntDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<SpecLookupEditDto?> reply = await _lookupService.GetEdit( request!.Payload!.Value );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "add-spec-lookup" )]
    public async Task<ActionResult<int>> Add( [FromBody] UserDataRequest<SpecLookupEditDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<int> reply = await _lookupService.Add( request!.Payload! );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "update-spec-lookup" )]
    public async Task<ActionResult<bool>> Update( [FromBody] UserDataRequest<SpecLookupEditDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<bool> reply = await _lookupService.Update( request!.Payload! );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "remove-spec-lookup" )]
    public async Task<ActionResult<bool>> Remove( [FromBody] UserDataRequest<IntDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<bool> reply = await _lookupService.Remove( request!.Payload!.Value );
        return GetReturnFromApi( reply );
    }
}