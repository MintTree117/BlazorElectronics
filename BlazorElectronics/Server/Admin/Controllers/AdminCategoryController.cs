using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin;
using BlazorElectronics.Shared.Admin.Categories;
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
    public async Task<ActionResult<ApiReply<CategoryViewDto?>>> GetCategoriesView( [FromBody] AdminRequest<object> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<Task<CategoryViewDto?>> action = _repository.GetCategoriesView;
        ApiReply<CategoryViewDto?> result = await TryExecuteAdminQuery<CategoryViewDto>( action );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<CategoryViewDto?>( result.Data ) )
            : Ok( new ApiReply<CategoryViewDto?>( NO_DATA_MESSAGE ) );
    }
    [HttpPost("get-category-edit")]
    public async Task<ActionResult<ApiReply<EditCategoryDto?>>> GetCategoryForEdit( [FromBody] AdminRequest<GetCategoryEditDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success || request.Dto is null )
            return BadRequest( sessionReply );

        Func<GetCategoryEditDto, Task<EditCategoryDto?>> action = _repository.GetEditCategory;
        ApiReply<EditCategoryDto?> result = await TryExecuteAdminQuery<EditCategoryDto>( action, request.Dto );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<EditCategoryDto?>( result.Data ) )
            : Ok( new ApiReply<EditCategoryDto?>( NO_DATA_MESSAGE ) );
    }
    [HttpPost( "add-category" )]
    public async Task<ActionResult<ApiReply<EditCategoryDto?>>> AddCategory( [FromBody] AdminRequest<AddCategoryDto> request )
    {
        Logger.LogError( "Hit controller" );
        
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Logger.LogError( "Validated admin" );
        
        Func<AddCategoryDto, Task<EditCategoryDto?>> action = _repository.InsertCategory;
        ApiReply<EditCategoryDto?> result = await TryExecuteAdminQuery<EditCategoryDto>( action, request.Dto );
        
        Logger.LogError( "Add result " + result.Success );
        
        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<EditCategoryDto?>( result.Data ) )
            : Ok( new ApiReply<EditCategoryDto?>( result.Message ) );
    }
    [HttpPost( "update-category" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateCategory( [FromBody] AdminRequest<EditCategoryDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );
        
        Func<EditCategoryDto, Task<bool>> action = _repository.UpdateCategory;
        ApiReply<bool> result = await TryExecuteAdminTransaction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( "remove-category" )]
    public async Task<ActionResult<ApiReply<bool>>> RemoveCategory( [FromBody] AdminRequest<RemoveCategoryDto> request )
    {
        Logger.LogError( "Hit remove method" );
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );
        
        Func<RemoveCategoryDto, Task<bool>> action = _repository.DeleteCategory;
        ApiReply<bool> result = await TryExecuteAdminTransaction( action, request.Dto );

        Logger.LogError( "Remove result: " + result.Success + " " + request.Dto.CategoryTier + " " + request.Dto.CategoryId + " " + result.Message);
        
        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
}