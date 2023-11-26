using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Controllers;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin.Categories;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Admin.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminCategoryController : _AdminController
{
    readonly IAdminCategoryRepository _repository;
    
    public AdminCategoryController( ILogger<AdminCategoryController> logger, IUserAccountService userAccountService, ISessionService sessionService, IAdminCategoryRepository repository )
        : base( logger, userAccountService, sessionService )
    {
        _repository = repository;
    }

    [HttpPost( "get-categories-view" )]
    public async Task<ActionResult<ApiReply<CategoriesViewDto?>>> GetCategoriesView( [FromBody] UserRequest? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;
        
        try
        {
            CategoriesViewDto? result = await _repository.GetView();

            return result is not null
                ? Ok( new ApiReply<CategoriesViewDto>( result ) )
                : NotFound( NO_DATA_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
    [HttpPost("get-category-edit")]
    public async Task<ActionResult<ApiReply<CategoryEditDto?>>> GetCategoryForEdit( [FromBody] UserDataRequest<CategoryGetEditDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            CategoryEditDto? result = await _repository.GetEdit( request!.Payload! );

            return result is not null
                ? Ok( new ApiReply<CategoryEditDto>( result ) )
                : NotFound( NO_DATA_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
    [HttpPost( "add-category" )]
    public async Task<ActionResult<ApiReply<CategoryEditDto?>>> AddCategory( [FromBody] UserDataRequest<CategoryAddDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            CategoryEditDto? result = await _repository.Insert( request!.Payload! );

            return result is not null
                ? Ok( new ApiReply<CategoryEditDto>( result ) )
                : NotFound( NO_DATA_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
    [HttpPost( "update-category" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateCategory( [FromBody] UserDataRequest<CategoryEditDto>? request )
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
    [HttpPost( "remove-category" )]
    public async Task<ActionResult<ApiReply<bool>>> RemoveCategory( [FromBody] UserDataRequest<CategoryRemoveDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        try
        {
            bool result = await _repository.Delete( request!.Payload! );

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