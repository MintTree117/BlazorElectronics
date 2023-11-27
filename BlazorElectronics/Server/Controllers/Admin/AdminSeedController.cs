using BlazorElectronics.Server.Dtos.Categories;
using BlazorElectronics.Server.Dtos.SpecLookups;
using BlazorElectronics.Server.Repositories.Vendors;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Products;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.SpecLookups;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Users;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public class AdminSeedController : _AdminController
{
    readonly IVendorRepository _vendorRepository;
    readonly ICategoryService _categoryService;
    readonly ISpecLookupService _specService;
    readonly IUserSeedService _userSeedService;
    readonly IProductSeedService _productSeedService;

    public AdminSeedController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, IVendorRepository vendorRepository, ICategoryService categoryService, ISpecLookupService specService, IUserSeedService userSeedService, IProductSeedService productSeedService )
        : base( logger, userAccountService, sessionService )
    {
        _vendorRepository = vendorRepository;
        _categoryService = categoryService;
        _specService = specService;
        _userSeedService = userSeedService;
        _productSeedService = productSeedService;
    }

    [HttpPost( "seed-products" )]
    public async Task<ActionResult<ApiReply<bool>>> SeedProducts( [FromBody] UserDataRequest<IntDto> request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        VendorsResponse? vendors = await _vendorRepository.Get();
        ApiReply<CachedCategories?> categories = await _categoryService.GetCategoriesDto();
        ApiReply<CachedSpecLookupData?> lookups = await _specService.GetSpecLookups();
        var users = new List<int>();

        if ( vendors is null )
            return NotFound( "Vendors " + NOT_FOUND_MESSAGE );

        if ( !categories.Success || categories.Data is null )
            return NotFound( categories.Message );

        if ( !lookups.Success || lookups.Data is null )
            return NotFound( lookups.Message );

        ApiReply<bool> result = await _productSeedService.SeedProducts( request!.Payload!.Value, categories.Data, lookups.Data, vendors, users );

        return result.Success
            ? Ok( new ApiReply<bool>( true ) )
            : StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
    }

    [HttpPost( "seed-users" )]
    public async Task<ActionResult<ApiReply<bool>>> SeedUsers( [FromBody] UserDataRequest<IntDto> request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeAdmin( request );

        if ( authorized.HttpError is not null )
            return authorized.HttpError;

        ApiReply<bool> seedReply = await _userSeedService.SeedUsers( request!.Payload!.Value );

        return seedReply.Success
            ? Ok( new ApiReply<bool>( true ) )
            : StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
    }
}