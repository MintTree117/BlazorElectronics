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

    [HttpPost("get-edit")]
    public async Task<ActionResult<ApiReply<AddUpdateCategoryDto?>>> GetCategoryForEdit( [FromBody] AdminRequest<GetCategoryEditRequest> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success || request.Dto is null )
            return BadRequest( sessionReply );

        Func<GetCategoryEditRequest, Task<AddUpdateCategoryDto?>> action = _repository.GetEditCategory;
        ApiReply<AddUpdateCategoryDto?> result = await TryExecuteAdminQuery<AddUpdateCategoryDto>( action, request.Dto );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<AddUpdateCategoryDto?>( result.Data ) )
            : Ok( new ApiReply<AddUpdateCategoryDto?>( NO_DATA_MESSAGE ) );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<ApiReply<AddUpdateCategoryDto?>>> AddCategory( [FromBody] AdminRequest<AddUpdateCategoryDto> request )
    {
        Logger.LogError( "Hit controller" );
        
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Logger.LogError( "Validated admin" );
        
        Func<AddUpdateCategoryDto, Task<AddUpdateCategoryDto?>> action = _repository.AddCategory;
        ApiReply<AddUpdateCategoryDto?> result = await TryExecuteAdminQuery<AddUpdateCategoryDto>( action, request.Dto );
        
        Logger.LogError( "Add result " + result.Success );
        
        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<AddUpdateCategoryDto?>( result.Data ) )
            : Ok( new ApiReply<AddUpdateCategoryDto?>( result.Message ) );
    }
    [HttpPost( "update" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateCategory( [FromBody] AdminRequest<AddUpdateCategoryDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );
        
        Func<AddUpdateCategoryDto, Task<bool>> action = _repository.UpdateCategory;
        ApiReply<bool> result = await TryExecuteAdminTransaction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<ApiReply<bool>>> RemoveCategory( [FromBody] AdminRequest<DeleteCategoryDto> request )
    {

        Logger.LogError( "Hit remove method" );
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );
        
        Func<DeleteCategoryDto, Task<bool>> action = _repository.DeleteCategory;
        ApiReply<bool> result = await TryExecuteAdminTransaction( action, request.Dto );

        Logger.LogError( "Remove result: " + result.Success + " " + request.Dto.CategoryTier + " " + request.Dto.CategoryId + " " + result.Message);
        
        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
}