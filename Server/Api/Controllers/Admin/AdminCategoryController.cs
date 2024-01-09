using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Categories;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminCategoryController : _AdminController
{
    readonly ICategoryService _categoryService;
    
    public AdminCategoryController( ILogger<AdminCategoryController> logger, IUserAccountService userAccountService, ISessionService sessionService, ICategoryService categoryService )
        : base( logger, userAccountService, sessionService )
    {
        _categoryService = categoryService;
    }

    [HttpGet( "get-view" )]
    public async Task<ActionResult<List<CategoryViewDtoDto>?>> GetView()
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );
        
        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<CategoryViewDtoDto>?> reply = await _categoryService.GetView();
        return GetReturnFromReply( reply );
    }
    [HttpGet("get-edit")]
    public async Task<ActionResult<CategoryEditDto?>> GetEdit( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<CategoryEditDto?> reply = await _categoryService.GetEdit( itemId );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "add-bulk" )]
    public async Task<ActionResult<bool>> AddBulk( [FromBody] List<CategoryEditDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _categoryService.AddBulk( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "add" )]
    public async Task<ActionResult<int>> Add( [FromBody] CategoryEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _categoryService.Add( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "update" )]
    public async Task<ActionResult<bool>> Update( [FromBody] CategoryEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _categoryService.Update( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpDelete( "remove" )]
    public async Task<ActionResult<bool>> Remove( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _categoryService.Remove( itemId );
        return GetReturnFromReply( reply );
    }
}