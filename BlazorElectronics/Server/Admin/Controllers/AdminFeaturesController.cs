using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Controllers;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin.Features;
using BlazorElectronics.Shared.SpecLookups;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Admin.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminFeaturesController : _AdminController
{
    readonly IAdminFeaturesRepository _repository;
    
    public AdminFeaturesController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, IAdminFeaturesRepository repository )
        : base( logger, userAccountService, sessionService )
    {
        _repository = repository;
    }
    
    [HttpPost( "get-features-view" )]
    public async Task<ActionResult<ApiReply<List<FeaturesViewDto>>>> GetView( [FromBody] UserRequest? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            FeaturesViewDto? result = await _repository.GetView();

            return result is not null
                ? Ok( new ApiReply<FeaturesViewDto>( result ) )
                : NotFound( NO_DATA_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
    [HttpPost( "add-featured-product" )]
    public async Task<ActionResult<ApiReply<bool>>> AddProduct( [FromBody] UserDataRequest<FeaturedProductEditDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            bool result = await _repository.InsertFeaturedProduct( request!.Payload! );

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
    [HttpPost( "add-featured-deal" )]
    public async Task<ActionResult<ApiReply<bool>>> AddDeal( [FromBody] UserDataRequest<FeaturedDealEditDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            bool result = await _repository.InsertFeaturedDeal( request!.Payload! );

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
    [HttpPost( "update-featured-product" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateProduct( [FromBody] UserDataRequest<FeaturedProductEditDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            bool result = await _repository.UpdateFeaturedProduct( request!.Payload! );
            
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
    [HttpPost( "remove-featured-product" )]
    public async Task<ActionResult<ApiReply<bool>>> RemoveProduct( [FromBody] UserDataRequest<IdDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            bool result = await _repository.DeleteFeaturedProduct( request!.Payload!.Id );

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
    [HttpPost( "remove-featured-deal" )]
    public async Task<ActionResult<ApiReply<bool>>> RemoveDeal( [FromBody] UserDataRequest<IdDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;
        
        try
        {
            bool result = await _repository.DeleteFeaturedDeal( request!.Payload!.Id );

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