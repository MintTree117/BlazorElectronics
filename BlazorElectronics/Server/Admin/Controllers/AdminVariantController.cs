using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Controllers;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin.Variants;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Admin.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class AdminVariantController : _AdminController
{
    readonly IAdminVariantRepository _repository;
    
    public AdminVariantController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, IAdminVariantRepository repository )
        : base( logger, userAccountService, sessionService )
    {
        _repository = repository;
    }
    
    [HttpPost( "get-variants-view" )]
    public async Task<ActionResult<ApiReply<List<VariantViewDto>>>> GetView( [FromBody] UserRequest? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            List<VariantViewDto>? result = await _repository.GetView();

            return result is not null
                ? Ok( new ApiReply<List<VariantViewDto>>( result ) )
                : NotFound( NO_DATA_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
    [HttpPost( "get-variant-edit" )]
    public async Task<ActionResult<ApiReply<VariantEditDto>>> GetEdit( [FromBody] UserDataRequest<IdDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            VariantEditDto? result = await _repository.GetEdit( request!.Payload!.Id );

            return result is not null
                ? Ok( new ApiReply<VariantEditDto>( result ) )
                : NotFound( NO_DATA_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
    [HttpPost( "add-variant" )]
    public async Task<ActionResult<ApiReply<int>>> Add( [FromBody] UserDataRequest<VariantAddDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            int result = await _repository.Insert( request!.Payload! );

            return result > 0
                ? Ok( new ApiReply<int>( result ) )
                : NotFound( NO_DATA_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
    [HttpPost( "update-variant" )]
    public async Task<ActionResult<ApiReply<bool>>> Update( [FromBody] UserDataRequest<VariantEditDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            bool result = await _repository.Update( request!.Payload! );

            return result
                ? Ok( new ApiReply<bool>( true ) )
                : NotFound( NO_DATA_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
    [HttpPost( "remove-variant" )]
    public async Task<ActionResult<ApiReply<bool>>> Remove( [FromBody] UserDataRequest<IdDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            bool result = await _repository.Delete( request!.Payload!.Id );

            return result
                ? Ok( new ApiReply<bool>( true ) )
                : NotFound( NO_DATA_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
}