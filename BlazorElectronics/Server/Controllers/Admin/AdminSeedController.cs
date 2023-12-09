using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Products;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Server.Services.Vendors;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Users;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public class AdminSeedController : _AdminController
{
    readonly IVendorService _vendorService;
    readonly ICategoryService _categoryService;
    readonly ISpecsService _lookupService;
    readonly IUserSeedService _userSeedService;
    readonly IProductSeedService _productSeedService;

    public AdminSeedController( 
        ILogger<UserController> logger, 
        IUserAccountService userAccountService, 
        ISessionService sessionService,
        ICategoryService categoryService,
        IUserSeedService userSeedService, 
        IProductSeedService productSeedService, 
        IVendorService vendorService, 
        ISpecsService lookupService )
        : base( logger, userAccountService, sessionService )
    {
        _categoryService = categoryService;
        _userSeedService = userSeedService;
        _productSeedService = productSeedService;
        _vendorService = vendorService;
        _lookupService = lookupService;
    }

    [HttpPost( "products" )]
    public async Task<ActionResult<bool>> SeedProducts( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<CategoriesResponse?> categories = await _categoryService.GetCategories();
        ServiceReply<SpecsResponse?> lookups = await _lookupService.GetSpecs( categories.Data.PrimaryIds );
        ServiceReply<VendorsResponse?> vendors = await _vendorService.GetVendors();
        ServiceReply<List<int>?> users = await UserAccountService.GetIds();
        
        Logger.LogError(users.Data.Count.ToString());

        if ( !categories.Success || !lookups.Success || !vendors.Success || !users.Success )
            return NotFound( NOT_FOUND_MESSAGE );

        ServiceReply<bool> seedResult = await _productSeedService.SeedProducts( request.Payload.Value, categories.Data!, lookups.Data!, vendors.Data!, users.Data! );

        return seedResult.Success
            ? Ok( seedResult.Data )
            : StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR + seedResult.Message );
    }

    [HttpPost( "users" )]
    public async Task<ActionResult<bool>> SeedUsers( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> seedReply = await _userSeedService.SeedUsers( request!.Payload!.Value );
        return GetReturnFromReply( seedReply );
    }
}