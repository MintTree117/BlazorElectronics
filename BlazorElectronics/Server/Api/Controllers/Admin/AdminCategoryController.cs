using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Users;
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

    [HttpPost( "get-view" )]
    public async Task<ActionResult<List<CategoryViewDtoDto>?>> GetCategoriesView( [FromBody] UserRequestDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );
        
        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<CategoryViewDtoDto>?> reply = await _service.GetCategoriesView();
        return GetReturnFromReply( reply );
    }
    [HttpPost("get-edit")]
    public async Task<ActionResult<CategoryEditDtoDto?>> GetCategoryForEdit( [FromBody] UserDataRequestDto<IntDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<CategoryEditDtoDto?> reply = await _service.GetCategoryEdit( requestDto.Payload.Value );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "add-bulk" )]
    public async Task<ActionResult<bool>> AddCategoriesBulk( [FromBody] UserDataRequestDto<List<CategoryEditDtoDto>> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );
        
        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _service.AddBulkCategories( requestDto.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<int>> AddCategory( [FromBody] UserDataRequestDto<CategoryEditDtoDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _service.AddCategory( requestDto.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "update" )]
    public async Task<ActionResult<bool>> UpdateCategory( [FromBody] UserDataRequestDto<CategoryEditDtoDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _service.UpdateCategory( requestDto.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<bool>> RemoveCategory( [FromBody] UserDataRequestDto<IntDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _service.RemoveCategory( requestDto.Payload.Value );
        return GetReturnFromReply( reply );
    }
}