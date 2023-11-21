using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Dtos.Users;
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
    public async Task<ActionResult<ApiReply<bool>>> GetSpecLookupView( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<object?>> validateReply = await ValidateAdminRequest<object>( apiRequest );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<Task<SpecsViewDto?>> action = _lookupRepository.GetSpecsView;
        ApiReply<SpecsViewDto?> result = await TryExecuteAdminRepoQuery<SpecsViewDto>( action );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<SpecsViewDto?>( result.Data ) )
            : Ok( new ApiReply<SpecsViewDto?>( result.Message ) );
    }
    [HttpPost( "get-spec-lookup-edit" )]
    public async Task<ActionResult<ApiReply<bool>>> GetSpecLookupEdit( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<GetSpecLookupEditDto?>> validateReply = await ValidateAdminRequest<GetSpecLookupEditDto>( apiRequest );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<GetSpecLookupEditDto, Task<EditSpecLookupDto?>> action = _lookupRepository.GetSpecEdit;
        ApiReply<EditSpecLookupDto?> result = await TryExecuteAdminRepoQuery<EditSpecLookupDto>( action );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<EditSpecLookupDto?>( result.Data ) )
            : Ok( new ApiReply<EditSpecLookupDto?>( result.Message ) );
    }
    [HttpPost( "add-spec-lookup" )]
    public async Task<ActionResult<ApiReply<EditSpecLookupDto?>>> AddSpecLookup( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<EditSpecLookupDto?>> validateReply = await ValidateAdminRequest<EditSpecLookupDto>( apiRequest );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<AddSpecLookupDto, Task<EditSpecLookupDto?>> action = _lookupRepository.Insert;
        ApiReply<EditSpecLookupDto?> result = await TryExecuteAdminRepoQuery<EditSpecLookupDto>( action, validateReply.Data );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<EditSpecLookupDto?>( result.Data ) )
            : Ok( new ApiReply<EditSpecLookupDto?>( result.Message ) );
    }
    [HttpPost( "update-spec-lookup" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateSpecLookup( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<EditSpecLookupDto?>> validateReply = await ValidateAdminRequest<EditSpecLookupDto>( apiRequest );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<EditSpecLookupDto, Task<bool>> action = _lookupRepository.Update;
        ApiReply<bool> result = await TryExecuteAdminRepoTransaction( action, validateReply.Data );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( "remove-spec-lookup" )]
    public async Task<ActionResult<ApiReply<bool>>> DeleteSpecLookup( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<RemoveSpecLookupDto?>> validateReply = await ValidateAdminRequest<RemoveSpecLookupDto>( apiRequest );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<RemoveSpecLookupDto, Task<bool>> action = _lookupRepository.Delete;
        ApiReply<bool> result = await TryExecuteAdminRepoTransaction( action, validateReply.Data );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
}