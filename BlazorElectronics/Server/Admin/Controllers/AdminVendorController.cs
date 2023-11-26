using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Controllers;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin.Vendors;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Admin.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminVendorController : _AdminController
{
    readonly IAdminVendorRepository _repository;

    public AdminVendorController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, IAdminVendorRepository repository )
        : base( logger, userAccountService, sessionService )
    {
        _repository = repository;
    }
    
    [HttpPost( "get-vendors-view" )]
    public async Task<ActionResult<ApiReply<VendorsViewDto>>> GetView( [FromBody] UserRequest? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            VendorsViewDto? result = await _repository.GetView();

            return result is not null
                ? Ok( new ApiReply<VendorsViewDto>( result ) )
                : NotFound( NO_DATA_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
    [HttpPost( "get-vendor-edit" )]
    public async Task<ActionResult<ApiReply<VendorEditDto>>> GetEdit( [FromBody] UserDataRequest<IdDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            VendorEditDto? result = await _repository.GetEdit( request!.Payload!.Id );

            return result is not null
                ? Ok( new ApiReply<VendorEditDto>( result ) )
                : NotFound( NO_DATA_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
    [HttpPost( "add-vendor" )]
    public async Task<ActionResult<ApiReply<int>>> Add( [FromBody] UserDataRequest<VendorEditDto>? request )
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
    [HttpPost( "update-vendor" )]
    public async Task<ActionResult<ApiReply<bool>>> Update( [FromBody] UserDataRequest<VendorEditDto>? request )
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
    [HttpPost( "remove-vendor" )]
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