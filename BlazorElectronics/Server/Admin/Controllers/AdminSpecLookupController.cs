using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Controllers;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin.SpecLookups;
using BlazorElectronics.Shared.SpecLookups;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Admin.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class AdminSpecLookupController : _AdminController
{
    readonly IAdminSpecLookupRepository _repository;

    public AdminSpecLookupController( ILogger<AdminSpecLookupController> logger, IUserAccountService userAccountService, ISessionService sessionService, IAdminSpecLookupRepository repository )
        : base( logger, userAccountService, sessionService )
    {
        _repository = repository;
    }
    
    [HttpPost( "get-spec-lookup-view" )]
    public async Task<ActionResult<ApiReply<List<SpecLookupViewDto>>>> GetView( [FromBody] UserRequest? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            List<SpecLookupViewDto>? result = await _repository.GetView();

            return result is not null
                ? Ok( new ApiReply<List<SpecLookupViewDto>>( result ) )
                : NotFound( NO_DATA_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
    [HttpPost( "get-spec-lookup-edit" )]
    public async Task<ActionResult<ApiReply<SpecLookupEditDto>>> GetEdit( [FromBody] UserDataRequest<IdDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;
        
        try
        {
            SpecLookupEditDto? result = await _repository.GetEdit( request!.Payload!.Id );

            return result is not null
                ? Ok( new ApiReply<SpecLookupEditDto>( result ) )
                : NotFound( NO_DATA_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
    [HttpPost( "add-spec-lookup" )]
    public async Task<ActionResult<ApiReply<int>>> Add( [FromBody] UserDataRequest<SpecLookupEditDto>? request )
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
    [HttpPost( "update-spec-lookup" )]
    public async Task<ActionResult<ApiReply<bool>>> Update( [FromBody] UserDataRequest<SpecLookupEditDto>? request )
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
    [HttpPost( "remove-spec-lookup" )]
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