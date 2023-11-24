using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Controllers;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin.Variants;
using BlazorElectronics.Shared.Inbound.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Admin.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class AdminVariantController : _AdminController
{
    readonly IAdminVariantRepository _variantRepository;
    
    public AdminVariantController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, IAdminVariantRepository variantRepository )
        : base( logger, userAccountService, sessionService )
    {
        _variantRepository = variantRepository;
    }

    [HttpPost( "get-variants-view" )]
    public async Task<ActionResult<ApiReply<VariantsViewDto>>> GetView( [FromBody] UserRequest? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<Task<VariantsViewDto?>> action = _variantRepository.GetView;
        ApiReply<VariantsViewDto?> result = await TryExecuteAdminRepoQuery<VariantsViewDto>( action );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<VariantsViewDto?>( result.Data ) )
            : Ok( new ApiReply<VariantsViewDto?>( result.Message ) );
    }
    [HttpPost( "get-variant-edit" )]
    public async Task<ActionResult<ApiReply<VariantEditDto>>> GetEdit( [FromBody] UserDataRequest<VariantGetEditDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<VariantGetEditDto, Task<VariantEditDto?>> action = _variantRepository.GetEdit;
        ApiReply<VariantEditDto?> result = await TryExecuteAdminRepoQuery<VariantEditDto>( action, request!.Payload );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<VariantEditDto?>( result.Data ) )
            : Ok( new ApiReply<VariantEditDto?>( result.Message ) );
    }
    [HttpPost( "add-variant" )]
    public async Task<ActionResult<ApiReply<int>>> Add( [FromBody] UserDataRequest<VariantAddDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Logger.LogError( "hit" );
        
        Func<VariantAddDto, Task<int>> action = _variantRepository.Insert;
        ApiReply<int> result = await TryExecuteAdminRepoQuery<int>( action, request!.Payload );

        return result.Success
            ? Ok( new ApiReply<int>( result.Data ) )
            : Ok( new ApiReply<int>( result.Message ) );
    }
    [HttpPost( "update-variant" )]
    public async Task<ActionResult<ApiReply<bool>>> Update( [FromBody] UserDataRequest<VariantEditDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<VariantEditDto, Task<bool>> action = _variantRepository.Update;
        ApiReply<bool> result = await TryExecuteAdminRepoTransaction( action, request!.Payload );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( "remove-variant" )]
    public async Task<ActionResult<ApiReply<bool>>> Remove( [FromBody] UserDataRequest<VariantRemoveDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<VariantRemoveDto, Task<bool>> action = _variantRepository.Delete;
        ApiReply<bool> result = await TryExecuteAdminRepoTransaction( action, request!.Payload );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
}