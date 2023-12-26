using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminSeedController : _AdminController
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

    [HttpPut( "products" )]
    public async Task<ActionResult<bool>> SeedProducts( int amount )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<CategoryData?> categoryReply = await _categoryService.GetCategoryData();
        
        if ( !categoryReply.Success || categoryReply.Data is null )
            return GetReturnFromReply( categoryReply );
        
        ServiceReply<LookupSpecsDto?> lookupReply = await _lookupService.GetSpecs();

        if ( !lookupReply.Success || lookupReply.Data is null )
            return GetReturnFromReply( lookupReply );
        
        ServiceReply<VendorsDto?> vendorReply = await _vendorService.GetVendors();

        if ( !vendorReply.Success || vendorReply.Data is null )
            return GetReturnFromReply( vendorReply );

        ServiceReply<bool> seedReply = await _productSeedService.SeedProducts( amount, categoryReply.Data, lookupReply.Data, vendorReply.Data );

        return GetReturnFromReply( seedReply );
    }
    [HttpPut( "reviews" )]
    public async Task<ActionResult<bool>> SeedReviews( int amount )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<int>> productReply = await _productService.GetAllIds();

        if ( !productReply.Success || productReply.Data is null )
            return GetReturnFromReply( productReply );

        ServiceReply<List<int>?> userReply = await _userSerice.GetAllIds();

        if ( !userReply.Success || userReply.Data is null )
            return GetReturnFromReply( userReply );
        
        Logger.LogError( productReply.Data.Count.ToString() );

        ServiceReply<bool> seedReply = await _reviewSeedService.SeedReviews( amount, productReply.Data, userReply.Data );

        if ( !seedReply.Success )
            return GetReturnFromReply( seedReply );

        ServiceReply<bool> updateReply = await _productService.UpdateProductsReviewData();
        return GetReturnFromReply( updateReply );
    }
    [HttpPut( "users" )]
    public async Task<ActionResult<bool>> SeedUsers( int amount )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> seedReply = await _userSeedService.SeedUsers( amount );
        return GetReturnFromReply( seedReply );
    }
}