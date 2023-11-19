using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin;
using BlazorElectronics.Shared.Admin.Products;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Admin.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class AdminProductController : _AdminController
{
    readonly IAdminProductRepository _repository;
    
    public AdminProductController( ILogger<AdminProductController> logger, IUserAccountService userAccountService, ISessionService sessionService, IAdminProductRepository repository )
        : base( logger, userAccountService, sessionService )
    {
        _repository = repository;
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> AddProduct( [FromBody] AdminRequest<AddUpdateProductDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<AddUpdateProductDto, Task<bool>> action = _repository.AddProduct;
        ApiReply<bool> result = await TryExecuteAdminTransaction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> UpdateProduct( [FromBody] AdminRequest<AddUpdateProductDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<AddUpdateProductDto, Task<bool>> action = _repository.UpdateProduct;
        ApiReply<bool> result = await TryExecuteAdminTransaction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> DeleteProduct( [FromBody] AdminRequest<RemoveProductDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<RemoveProductDto, Task<bool>> action = _repository.RemoveProduct;
        ApiReply<bool> result = await TryExecuteAdminTransaction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
}