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
    public async Task<ActionResult<ApiReply<CategoriesViewDto?>>> GetCategoriesView( [FromBody] UserRequest? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<Task<CategoriesViewDto?>> action = _repository.GetView;
        ApiReply<CategoriesViewDto?> result = await TryExecuteAdminRepoQuery<CategoriesViewDto>( action );
        
        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<CategoriesViewDto?>( result.Data ) )
            : Ok( new ApiReply<CategoriesViewDto?>( NO_DATA_MESSAGE ) );
    }
    [HttpPost("get-category-edit")]
    public async Task<ActionResult<ApiReply<CategoryEditDto?>>> GetCategoryForEdit( [FromBody] UserDataRequest<CategoryGetEditDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<CategoryGetEditDto, Task<CategoryEditDto?>> action = _repository.GetEdit;
        ApiReply<CategoryEditDto?> result = await TryExecuteAdminRepoQuery<CategoryEditDto>( action, request!.Payload! );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<CategoryEditDto?>( result.Data ) )
            : Ok( new ApiReply<CategoryEditDto?>( NO_DATA_MESSAGE ) );
    }
    [HttpPost( "add-category" )]
    public async Task<ActionResult<ApiReply<CategoryEditDto?>>> AddCategory( [FromBody] UserDataRequest<CategoryAddDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        Func<CategoryAddDto, Task<CategoryEditDto?>> action = _repository.Insert;
        ApiReply<CategoryEditDto?> result = await TryExecuteAdminRepoQuery<CategoryEditDto>( action, request!.Payload );

        return result is { Success: true, Data: not null }
            ? Ok( new ApiReply<CategoryEditDto?>( result.Data ) )
            : Ok( new ApiReply<CategoryEditDto?>( result.Message ) );
    }
    [HttpPost( "update-category" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateCategory( [FromBody] UserDataRequest<CategoryEditDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );
        
        Logger.LogError( request.Payload.Type.ToString() );
        
        Func<CategoryEditDto, Task<bool>> action = _repository.Update;
        ApiReply<bool> result = await TryExecuteAdminRepoTransaction( action, request!.Payload );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( "remove-category" )]
    public async Task<ActionResult<ApiReply<bool>>> RemoveCategory( [FromBody] UserDataRequest<CategoryRemoveDto>? request )
    {
        ApiReply<int> validateReply = await ValidateAdminRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );
        
        Func<CategoryRemoveDto, Task<bool>> action = _repository.Delete;
        ApiReply<bool> result = await TryExecuteAdminRepoTransaction( action, request!.Payload );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
}