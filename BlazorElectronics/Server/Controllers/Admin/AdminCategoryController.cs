using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminCategoryController : _AdminController
{
    readonly ICategoryService _service;
    
    public AdminCategoryController( ILogger<AdminCategoryController> logger, IUserAccountService userAccountService, ISessionService sessionService, ICategoryService service )
        : base( logger, userAccountService, sessionService )
    {
        _service = service;
    }

    [HttpPost( "get-view" )]
    public async Task<ActionResult<List<CategoryView>?>> GetCategoriesView( [FromBody] UserRequest request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );
        
        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<CategoryView>?> reply = await _service.GetCategoriesView();
        return GetReturnFromReply( reply );
    }
    [HttpPost("get-edit")]
    public async Task<ActionResult<CategoryEdit?>> GetCategoryForEdit( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<CategoryEdit?> reply = await _service.GetCategoryEdit( request.Payload.Value );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "add-bulk" )]
    public async Task<ActionResult<bool>> AddCategoriesBulk( [FromBody] UserDataRequest<List<CategoryEdit>> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );
        
        Logger.LogError( "hit" );
        
        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );
        
        Logger.LogError( "validated" );
        
        ServiceReply<bool> reply = await _service.AddBulkCategories( request.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<int>> AddCategory( [FromBody] UserDataRequest<CategoryEdit> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _service.AddCategory( request.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "update" )]
    public async Task<ActionResult<bool>> UpdateCategory( [FromBody] UserDataRequest<CategoryEdit> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _service.UpdateCategory( request.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<bool>> RemoveCategory( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _service.RemoveCategory( request.Payload.Value );
        return GetReturnFromReply( reply );
    }
}