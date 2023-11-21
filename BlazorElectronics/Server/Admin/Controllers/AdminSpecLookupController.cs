using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin;
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
    public async Task<ActionResult<ApiReply<bool>>> GetSpecLookupView( [FromBody] UserApiRequest request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<Task<SpecsViewDto?>> action = _lookupRepository.GetSpecsView;
        ApiReply<SpecsViewDto?> result = await TryExecuteAdminQuery<SpecsViewDto>( action );

        return result.Success && result.Data is not null
            ? Ok( new ApiReply<SpecsViewDto?>( result.Data ) )
            : Ok( new ApiReply<SpecsViewDto?>( result.Message ) );
    }
    [HttpPost( "add-spec-lookup" )]
    public async Task<ActionResult<ApiReply<EditSpecLookupDto?>>> AddSpecLookup( [FromBody] AdminRequest<AddSpecLookupDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<AddSpecLookupDto, Task<EditSpecLookupDto?>> action = _lookupRepository.Insert;
        ApiReply<EditSpecLookupDto?> result = await TryExecuteAdminQuery<EditSpecLookupDto>( action, request.Dto );

        return result.Success && result.Data is not null
            ? Ok( new ApiReply<EditSpecLookupDto?>( result.Data ) )
            : Ok( new ApiReply<EditSpecLookupDto?>( result.Message ) );
    }
    [HttpPost( "update-spec-lookup" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateSpecLookup( [FromBody] AdminRequest<EditSpecLookupDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<EditSpecLookupDto, Task<bool>> action = _lookupRepository.Update;
        ApiReply<bool> result = await TryExecuteAdminTransaction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( "remove-spec-lookup" )]
    public async Task<ActionResult<ApiReply<bool>>> DeleteSpecLookup( [FromBody] AdminRequest<RemoveSpecLookupDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<RemoveSpecLookupDto, Task<bool>> action = _lookupRepository.Delete;
        ApiReply<bool> result = await TryExecuteAdminTransaction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
}