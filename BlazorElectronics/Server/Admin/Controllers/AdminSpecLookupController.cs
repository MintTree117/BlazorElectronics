using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin.Specs;
using BlazorElectronics.Shared.Inbound.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Admin.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class AdminSpecLookupController : _AdminController
{
    readonly IAdminSpecLookupRepository _lookupRepository;

    public AdminSpecLookupController( ILogger<AdminSpecLookupController> logger, IUserAccountService userAccountService, ISessionService sessionService, IAdminSpecLookupRepository lookupRepository )
        : base( logger, userAccountService, sessionService )
    {
        _lookupRepository = lookupRepository;
    }

    [HttpPost( "get-spec-lookup-view" )]
    public async Task<ActionResult<ApiReply<bool>>> GetSpecLookupView( [FromBody] UserRequest? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<Task<SpecsViewDto?>> action = _lookupRepository.GetSpecsView;
        ApiReply<SpecsViewDto?> result = await TryExecuteAdminRepoQuery<SpecsViewDto>( action );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<SpecsViewDto?>( result.Data ) )
            : Ok( new ApiReply<SpecsViewDto?>( result.Message ) );
    }
    [HttpPost( "get-spec-lookup-edit" )]
    public async Task<ActionResult<ApiReply<EditSpecLookupDto>>> GetSpecLookupEdit( [FromBody] UserDataRequest<GetSpecLookupEditDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<GetSpecLookupEditDto, Task<EditSpecLookupDto?>> action = _lookupRepository.GetSpecEdit;
        ApiReply<EditSpecLookupDto?> result = await TryExecuteAdminRepoQuery<EditSpecLookupDto>( action, request!.Payload );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<EditSpecLookupDto?>( result.Data ) )
            : Ok( new ApiReply<EditSpecLookupDto?>( result.Message ) );
    }
    [HttpPost( "add-spec-lookup" )]
    public async Task<ActionResult<ApiReply<int>>> AddSpecLookup( [FromBody] UserDataRequest<EditSpecLookupDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );
        
        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<EditSpecLookupDto, Task<int>> action = _lookupRepository.Insert;
        ApiReply<int> result = await TryExecuteAdminRepoQuery<int>( action, request!.Payload );

        return result.Success
            ? Ok( new ApiReply<int>( result.Data ) )
            : Ok( new ApiReply<int>( result.Message ) );
    }
    [HttpPost( "update-spec-lookup" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateSpecLookup( [FromBody] UserDataRequest<EditSpecLookupDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<EditSpecLookupDto, Task<bool>> action = _lookupRepository.Update;
        ApiReply<bool> result = await TryExecuteAdminRepoTransaction( action, request!.Payload );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( "remove-spec-lookup" )]
    public async Task<ActionResult<ApiReply<bool>>> DeleteSpecLookup( [FromBody] UserDataRequest<RemoveSpecLookupDto>? request )
    {
        Logger.LogError( "hit" );
        
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<RemoveSpecLookupDto, Task<bool>> action = _lookupRepository.Delete;
        ApiReply<bool> result = await TryExecuteAdminRepoTransaction( action, request!.Payload );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
}