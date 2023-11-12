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
    
    public AdminCategoryController( ILogger logger, IUserAccountService userAccountService, ISessionService sessionService, IAdminCategoryRepository repository )
        : base( logger, userAccountService, sessionService )
    {
        _repository = repository;
    }

    [HttpPost( $"{AdminControllerRoutes.AddCategory}" )]
    public async Task<ActionResult<ApiReply<bool>>> AddCategory( [FromBody] AdminRequest<AddUpdateCategoryDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );
        
        Func<AddUpdateCategoryDto, Task<bool>> action = _repository.AddCategory;
        ApiReply<bool> result = await TryExecuteAdminAction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( $"{AdminControllerRoutes.AddCategory}" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateCategory( [FromBody] AdminRequest<AddUpdateCategoryDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );
        
        Func<AddUpdateCategoryDto, Task<bool>> action = _repository.UpdateCategory;
        ApiReply<bool> result = await TryExecuteAdminAction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( $"{AdminControllerRoutes.AddCategory}" )]
    public async Task<ActionResult<ApiReply<bool>>> DeleteCategory( [FromBody] AdminRequest<DeleteCategoryDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );
        
        Func<DeleteCategoryDto, Task<bool>> action = _repository.DeleteCategory;
        ApiReply<bool> result = await TryExecuteAdminAction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
}