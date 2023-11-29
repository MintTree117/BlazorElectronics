using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Admin.Categories;
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

    [HttpPost( "get-categories-view" )]
    public async Task<ActionResult<CategoriesViewDto?>> GetCategoriesView( [FromBody] UserRequest? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );
        
        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<CategoriesViewDto?> reply = await _service.GetCategoriesView();
        return GetReturnFromApi( reply );
    }
    [HttpPost("get-category-edit")]
    public async Task<ActionResult<CategoryEditDto?>> GetCategoryForEdit( [FromBody] UserDataRequest<CategoryGetEditDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<CategoryEditDto?> reply = await _service.GetCategoryEdit(  request!.Payload!);
        return GetReturnFromApi( reply );
    }
    [HttpPost( "add-category" )]
    public async Task<ActionResult<CategoryEditDto?>> AddCategory( [FromBody] UserDataRequest<CategoryAddDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<CategoryEditDto?> reply = await _service.AddCategory( request!.Payload! );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "update-category" )]
    public async Task<ActionResult<bool>> UpdateCategory( [FromBody] UserDataRequest<CategoryEditDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<bool> reply = await _service.UpdateCategory( request!.Payload! );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "remove-category" )]
    public async Task<ActionResult<bool>> RemoveCategory( [FromBody] UserDataRequest<CategoryRemoveDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<bool> reply = await _service.RemoveCategory( request!.Payload! );
        return GetReturnFromApi( reply );
    }
}