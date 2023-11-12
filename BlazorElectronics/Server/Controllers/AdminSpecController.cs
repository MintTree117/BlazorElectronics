using BlazorElectronics.Server.Repositories.Specs;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Inbound.Admin;
using BlazorElectronics.Shared.Inbound.Admin.SpecLookups;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class AdminSpecController : AdminController
{
    readonly IAdminSpecLookupRepository _repository;

    public AdminSpecController( ILogger logger, IUserAccountService userAccountService, ISessionService sessionService, IAdminSpecLookupRepository repository )
        : base( logger, userAccountService, sessionService )
    {
        _repository = repository;
    }

    [HttpPost($"{AdminControllerRoutes.AddSpecSingle}")]
    public async Task<ActionResult<ApiReply<bool>>> AddSpecLookupSingle( [FromBody] AddSpecSingleRequest request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<SingleSpecLookupType, string, Dictionary<int, object>?, List<int>?, bool?, Task<bool>> action = _repository.AddSpecSingle;

        ApiReply<bool> result = await TryExecuteAdminAction(
            action, request.SpecType, request.SpecName, request.FilterValuesById, request.PrimaryCategories, request.IsGlobal );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( $"{AdminControllerRoutes.UpdateSpecSingle}" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateSpecLookupSingle( [FromBody] UpdateSpecSingleRequest request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<SingleSpecLookupType, int, string, Dictionary<int, object>?, List<int>?, bool?, Task<bool>> action = _repository.UpdateSpecSingle;

        ApiReply<bool> result = await TryExecuteAdminAction(
            action, request.SpecType, request.SpecId, request.SpecName, request.FilterValuesById, request.PrimaryCategories, request.IsGlobal );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( $"{AdminControllerRoutes.DeleteSpecSingle}" )]
    public async Task<ActionResult<ApiReply<bool>>> DeleteSpecLookupSingle( [FromBody] DeleteSpecSingleRequest request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<SingleSpecLookupType, int, Task<bool>> action = _repository.DeleteSpecSingle;

        ApiReply<bool> result = await TryExecuteAdminAction(
            action, request.SpecType, request.SpecId );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }

    [HttpPost( $"{AdminControllerRoutes.AddSpecMulti}" )]
    public async Task<ActionResult<ApiReply<bool>>> AddSpecLookupMulti( [FromBody] AddSpecMultiTableRequest request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<string, List<string>?, List<int>?, bool?, Task<bool>> action = _repository.AddSpecMultiTable;

        ApiReply<bool> result = await TryExecuteAdminAction(
            action, request.TableName, request.MultiValues, request.PrimaryCategories, request.IsGlobal );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( $"{AdminControllerRoutes.UpdateSpecMuti}" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateSpecLookupMulti( [FromBody] UpdateSpecMultiTableRequest request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<int, string, List<string>?, List<int>?, bool?, Task<bool>> action = _repository.UpdateSpecMultiTable;

        ApiReply<bool> result = await TryExecuteAdminAction(
            action, request.TableId, request.TableName, request.MultiValues, request.PrimaryCategories, request.IsGlobal );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
    [HttpPost( $"{AdminControllerRoutes.DeleteSpecMulti}" )]
    public async Task<ActionResult<ApiReply<bool>>> DeleteSpecLookupMulti( [FromBody] DeleteSpecMultiTableRequest request )
    {
        ApiReply<bool> sessionReply = await ValidateAdminRequest( request.SessionApiRequest, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return BadRequest( sessionReply );

        Func<int, string, Task<bool>> action = _repository.DeleteSpecMultiTable;

        ApiReply<bool> result = await TryExecuteAdminAction(
            action, request.TableId, request.TableName );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( result.Message ) );
    }
}