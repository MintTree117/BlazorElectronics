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
    readonly ISpecLookupsService _lookupService;
    readonly IProductServiceAdmin _productServiceAdmin;
    readonly IUserAccountService _userSerice;

    readonly IProductSeedService _productSeedService;
    readonly IReviewSeedService _reviewSeedService;
    readonly IUserSeedService _userSeedService;

    public AdminSeedController( 
        ILogger<_UserController> logger, 
        IUserAccountService userAccountService, 
        ISessionService sessionService,
        ICategoryService categoryService,
        IVendorService vendorService,
        ISpecLookupsService lookupService,
        IProductServiceAdmin productServiceAdmin,
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
        _productServiceAdmin = productServiceAdmin;
    }

    [HttpPut( "products" )]
    public async Task<ActionResult<bool>> SeedProducts( int amount )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<CategoryDataDto?> categoryReply = await _categoryService.GetData();
        
        if ( !categoryReply.Success || categoryReply.Payload is null )
            return GetReturnFromReply( categoryReply );
        
        ServiceReply<LookupSpecsDto?> lookupReply = await _lookupService.Get();

        if ( !lookupReply.Success || lookupReply.Payload is null )
            return GetReturnFromReply( lookupReply );
        
        ServiceReply<VendorsDto?> vendorReply = await _vendorService.Get();

        if ( !vendorReply.Success || vendorReply.Payload is null )
            return GetReturnFromReply( vendorReply );

        ServiceReply<bool> seedReply = await _productSeedService.SeedProducts( amount, categoryReply.Payload, lookupReply.Payload, vendorReply.Payload );

        return GetReturnFromReply( seedReply );
    }
    [HttpPut( "reviews" )]
    public async Task<ActionResult<bool>> SeedReviews( int amount )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<int>> productReply = await _productServiceAdmin.GetAllIds();

        if ( !productReply.Success || productReply.Payload is null )
            return GetReturnFromReply( productReply );

        ServiceReply<List<int>?> userReply = await _userSerice.GetAllIds();

        if ( !userReply.Success || userReply.Payload is null )
            return GetReturnFromReply( userReply );
        
        Logger.LogError( productReply.Payload.Count.ToString() );

        ServiceReply<bool> seedReply = await _reviewSeedService.SeedReviews( amount, productReply.Payload, userReply.Payload );

        if ( !seedReply.Success )
            return GetReturnFromReply( seedReply );

        ServiceReply<bool> updateReply = await _productServiceAdmin.UpdateProductsReviewData();
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