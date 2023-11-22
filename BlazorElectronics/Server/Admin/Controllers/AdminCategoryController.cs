using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin.Categories;
using BlazorElectronics.Shared.Inbound.Users;
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
    public async Task<ActionResult<ApiReply<CategoryViewDto?>>> GetCategoriesView( [FromBody] UserRequest? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<Task<CategoryViewDto?>> action = _repository.GetCategoriesView;
        ApiReply<CategoryViewDto?> result = await TryExecuteAdminRepoQuery<CategoryViewDto>( action );
        
        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<CategoryViewDto?>( result.Data ) )
            : Ok( new ApiReply<CategoryViewDto?>( NO_DATA_MESSAGE ) );
    }
    [HttpPost("get-category-edit")]
    public async Task<ActionResult<ApiReply<EditCategoryDto?>>> GetCategoryForEdit( [FromBody] UserDataRequest<GetCategoryEditDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<GetCategoryEditDto, Task<EditCategoryDto?>> action = _repository.GetEditCategory;
        ApiReply<EditCategoryDto?> result = await TryExecuteAdminRepoQuery<EditCategoryDto>( action, request!.Payload! );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<EditCategoryDto?>( result.Data ) )
            : Ok( new ApiReply<EditCategoryDto?>( NO_DATA_MESSAGE ) );
    }
    [HttpPost( "add-category" )]
    public async Task<ActionResult<ApiReply<EditCategoryDto?>>> AddCategory( [FromBody] UserDataRequest<AddCategoryDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<AddCategoryDto, Task<EditCategoryDto?>> action = _repository.InsertCategory;
        ApiReply<EditCategoryDto?> result = await TryExecuteAdminRepoQuery<EditCategoryDto>( action, request!.Payload );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<EditCategoryDto?>( result.Data ) )
            : Ok( new ApiReply<EditCategoryDto?>( result.Message ) );
    }
    [HttpPost( "update-category" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateCategory( [FromBody] UserDataRequest<EditCategoryDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );
        
        Logger.LogError( request.Payload.Type.ToString() );
        
        Func<EditCategoryDto, Task<bool>> action = _repository.UpdateCategory;
        ApiReply<bool> result = await TryExecuteAdminRepoTransaction( action, request!.Payload );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( "remove-category" )]
    public async Task<ActionResult<ApiReply<bool>>> RemoveCategory( [FromBody] UserDataRequest<RemoveCategoryDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );
        
        Func<RemoveCategoryDto, Task<bool>> action = _repository.DeleteCategory;
        ApiReply<bool> result = await TryExecuteAdminRepoTransaction( action, request!.Payload );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
}