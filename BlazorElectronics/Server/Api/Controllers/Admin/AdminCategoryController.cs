using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Categories;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

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

    [HttpGet( "get-view" )]
    public async Task<ActionResult<List<CategoryViewDtoDto>?>> GetCategoriesView()
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );
        
        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<CategoryViewDtoDto>?> reply = await _service.GetCategoriesView();
        return GetReturnFromReply( reply );
    }
    [HttpGet("get-edit")]
    public async Task<ActionResult<CategoryEditDto?>> GetCategoryForEdit( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<CategoryEditDto?> reply = await _service.GetCategoryEdit( itemId );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "add-bulk" )]
    public async Task<ActionResult<bool>> AddCategoriesBulk( [FromBody] List<CategoryEditDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _service.AddBulkCategories( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "add" )]
    public async Task<ActionResult<int>> AddCategory( [FromBody] CategoryEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _service.AddCategory( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "update" )]
    public async Task<ActionResult<bool>> UpdateCategory( [FromBody] CategoryEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _service.UpdateCategory( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpDelete( "remove" )]
    public async Task<ActionResult<bool>> RemoveCategory( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _service.RemoveCategory( itemId );
        return GetReturnFromReply( reply );
    }
}