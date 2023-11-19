using BlazorElectronics.Server.Admin.Repositories;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin;
using BlazorElectronics.Shared.Admin.SpecsMulti;
using BlazorElectronics.Shared.Admin.SpecsSingle;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Admin.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class AdminSpecController : _AdminController
{
    readonly IAdminSpecRepository _repository;

    public AdminSpecController( ILogger<AdminSpecController> logger, IUserAccountService userAccountService, ISessionService sessionService, IAdminSpecRepository repository )
        : base( logger, userAccountService, sessionService )
    {
        _repository = repository;
    }

    [HttpPost($"{AdminControllerRoutes.AddSpecSingle}")]
    public async Task<ActionResult<ApiReply<bool>>> AddSpecLookupSingle( [FromBody] AdminRequest<AddUpdateSpecSingleDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<AddUpdateSpecSingleDto, Task<bool>> action = _repository.AddSpecSingle;
        ApiReply<bool> result = await TryExecuteAdminTransaction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( $"{AdminControllerRoutes.UpdateSpecSingle}" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateSpecLookupSingle( [FromBody] AdminRequest<AddUpdateSpecSingleDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<AddUpdateSpecSingleDto, Task<bool>> action = _repository.UpdateSpecSingle;
        ApiReply<bool> result = await TryExecuteAdminTransaction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( $"{AdminControllerRoutes.DeleteSpecSingle}" )]
    public async Task<ActionResult<ApiReply<bool>>> DeleteSpecLookupSingle( [FromBody] AdminRequest<RemoveSpecSingleDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<RemoveSpecSingleDto, Task<bool>> action = _repository.RemoveSpecSingle;
        ApiReply<bool> result = await TryExecuteAdminTransaction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    
    [HttpPost( $"{AdminControllerRoutes.UpdateSpecMuti}" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateSpecLookupMulti( [FromBody] AdminRequest<UpdateSpecMultiDto> request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<UpdateSpecMultiDto, Task<bool>> action = _repository.UpdateSpecMultiTable;
        ApiReply<bool> result = await TryExecuteAdminTransaction( action, request.Dto );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
}