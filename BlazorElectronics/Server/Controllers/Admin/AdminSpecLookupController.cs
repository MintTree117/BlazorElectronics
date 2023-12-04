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
    
    [HttpPost( "get-view" )]
    public async Task<ActionResult<List<CrudView>?>> GetView( [FromBody] UserRequest request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<List<CrudView>?> reply = await _lookupService.GetView();
        return GetReturnFromApi( reply );
    }
    [HttpPost( "get-edit" )]
    public async Task<ActionResult<SpecLookupEdit?>> GetEdit( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<SpecLookupEdit?> reply = await _lookupService.GetEdit( request.Payload.Value );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<int>> Add( [FromBody] UserDataRequest<SpecLookupEdit> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<int> reply = await _lookupService.Add( request.Payload );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "update" )]
    public async Task<ActionResult<bool>> Update( [FromBody] UserDataRequest<SpecLookupEdit> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<bool> reply = await _lookupService.Update( request.Payload );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<bool>> Remove( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<bool> reply = await _lookupService.Remove( request.Payload.Value );
        return GetReturnFromApi( reply );
    }
}