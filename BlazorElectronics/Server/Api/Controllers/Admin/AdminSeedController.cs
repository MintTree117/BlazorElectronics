using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Users;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public class AdminSeedController : _AdminController
{
    readonly IVendorService _vendorService;
    readonly ICategoryService _categoryService;
    readonly ISpecsService _lookupService;
    readonly IProductService _productService;
    readonly IUserAccountService _userSerice;

    readonly IProductSeedService _productSeedService;
    readonly IReviewSeedService _reviewSeedService;
    readonly IUserSeedService _userSeedService;

    public AdminSeedController( 
        ILogger<UserController> logger, 
        IUserAccountService userAccountService, 
        ISessionService sessionService,
        ICategoryService categoryService,
        IVendorService vendorService,
        ISpecsService lookupService,
        IProductService productService,
        IProductSeedService productSeedService,
        IReviewSeedService reviewSeedService,
        IUserSeedService userSeedService, IUserAccountService userSerice )
        : base( logger, userAccountService, sessionService )
    {
        _categoryService = categoryService;
        _userSeedService = userSeedService;
        _userSerice = userSerice;
        _productSeedService = productSeedService;
        _reviewSeedService = reviewSeedService;
        _vendorService = vendorService;
        _lookupService = lookupService;
        _productService = productService;
    }

    [HttpPost( "products" )]
    public async Task<ActionResult<bool>> SeedProducts( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<CategoryData?> categoryReply = await _categoryService.GetCategoryData();
        
        if ( !categoryReply.Success || categoryReply.Data is null )
            return GetReturnFromReply( categoryReply );
        
        ServiceReply<SpecsResponse?> lookupReply = await _lookupService.GetSpecs( categoryReply.Data.PrimaryIds );

        if ( !lookupReply.Success || lookupReply.Data is null )
            return GetReturnFromReply( lookupReply );
        
        ServiceReply<VendorsResponse?> vendorReply = await _vendorService.GetVendors();

        if ( !vendorReply.Success || vendorReply.Data is null )
            return GetReturnFromReply( vendorReply );

        ServiceReply<bool> seedReply = await _productSeedService.SeedProducts( request.Payload.Value, categoryReply.Data, lookupReply.Data, vendorReply.Data );

        return GetReturnFromReply( seedReply );
    }

    [HttpPost( "reviews" )]
    public async Task<ActionResult<bool>> SeedReviews( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<int>> productReply = await _productService.GetAllIds();

        if ( !productReply.Success || productReply.Data is null )
            return GetReturnFromReply( productReply );

        ServiceReply<List<int>?> userReply = await _userSerice.GetAllIds();

        if ( !userReply.Success || userReply.Data is null )
            return GetReturnFromReply( userReply );

        ServiceReply<bool> seedReply = await _reviewSeedService.SeedReviews( request.Payload.Value, productReply.Data, userReply.Data );

        return GetReturnFromReply( seedReply );
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