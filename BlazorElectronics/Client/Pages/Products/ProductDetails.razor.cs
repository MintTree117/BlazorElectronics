using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Client.Services.Products;
using BlazorElectronics.Client.Services.Reviews;
using BlazorElectronics.Client.Services.Specs;
using BlazorElectronics.Client.Services.Vendors;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Reviews;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Products;

public partial class ProductDetails : PageView
{
    [Inject] public ICategoryServiceClient CategoryService { get; set; } = default!;
    [Inject] public IProductServiceClient ProductService { get; init; } = default!;
    [Inject] public IReviewServiceClient ReviewService { get; set; } = default!;
    [Inject] public ISpecServiceClient LookupService { get; set; } = default!;
    [Inject] public IVendorServiceClient VendorsService { get; set; } = default!;

    [Parameter] public string ProductId { get; set; } = string.Empty;
    
    ProductDto? _product;
    readonly List<CategoryFullDto> _categories = new();
    readonly Dictionary<string, string> _lookupSpecsAggregated = new();
    VendorDto _vendor = new();
    List<ProductReviewDto> _reviews = new();

    string _currentReviewSortOption = ReviewSortType.Date.ToString();
    List<string> _reviewSortOptions = Enum.GetNames<ReviewSortType>().ToList();
    int _selectedReviewSortOption = 0;
    readonly List<int> _reviewsPerPageOptions = new() { 5, 10, 15, 20 };
    int _reviewRows = 5;
    int _reviewPage = 1;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !int.TryParse( ProductId, out int id ) )
        {
            SetViewMessage( false, "Invalid Url!" );
            return;
        }
        
        ServiceReply<ProductDto?> productReply = await ProductService.GetProductDetails( id );
        PageIsLoaded = true;
        
        if ( !productReply.Success || productReply.Data is null )
        {
            SetViewMessage( false, $"Failed to fetch product details! {productReply.ErrorType} : {productReply.Message}" );
            PageIsLoaded = true;
            return;
        }

        _product = productReply.Data;

        ServiceReply<CategoryData?> categoriesReply = await CategoryService.GetCategories();

        if ( categoriesReply is { Success: true, Data: not null } )
            GetProductCategories( categoriesReply.Data.CategoriesById );

        ServiceReply<LookupSpecsDto?> lookupsReply = await LookupService.GetSpecLookups();

        if ( lookupsReply is { Success: true, Data: not null } )
            AggregateLookupSpecs( lookupsReply.Data );

        ServiceReply<VendorsDto?> vendorReply = await VendorsService.GetVendors();

        if ( vendorReply is { Success: true, Data: not null } )
        {
            if ( vendorReply.Data.VendorsById.TryGetValue( _product.VendorId, out VendorDto? vendor ) )
                _vendor = vendor;
        }

        await GetReviews( ReviewSortType.Date, 5, 1 );
    }
    public override bool PageIsReady()
    {
        return base.PageIsReady() && _product is not null;
    }

    void GetProductCategories( IReadOnlyDictionary<int, CategoryFullDto> categories )
    {
        if ( _product is null )
            return;
        
        foreach ( int id in _product.Categories )
        {
            if ( categories.TryGetValue( id, out CategoryFullDto? c ) )
                _categories.Add( c );
        }
    }
    void AggregateLookupSpecs( LookupSpecsDto lookups )
    {
        foreach ( KeyValuePair<int, List<int>> kvp in _product.LookupSpecs )
        {
            if ( !lookups.SpecsById.TryGetValue( kvp.Key, out LookupSpec? lookup ) )
                continue;

            List<string> valuesToAggregate = ( 
                from valueId in kvp.Value 
                where valueId >= 0 && valueId < lookup.Values.Count 
                select lookup.Values[ valueId ] )
                .ToList();

            _lookupSpecsAggregated.TryAdd( lookup.SpecName, string.Join( ", ", valuesToAggregate ) );
        }
    }
    async Task GetReviews( ReviewSortType sortType, int rows, int page )
    {
        if ( _product is null )
            return;
        
        GetProductReviewsDto dto = new()
        {
            ProductId = _product.Id,
            Rows = rows,
            Page = page,
            SortType = sortType
        };

        ServiceReply<List<ProductReviewDto>?> reply = await ReviewService.GetForProduct( dto );

        if ( !reply.Success )
        {
            Logger.LogError( reply.ErrorType.ToString() );
        }
        
        if ( reply is { Success: true, Data: not null } )
        {
            Logger.LogError( reply.Data.Count.ToString() );
            _reviews = reply.Data;
            StateHasChanged();
        }
    }

    async Task SelectReviewSort( int index )
    {
        if ( index < 0 || index >= _reviewSortOptions.Count )
            return;

        _selectedReviewSortOption = index;
        _currentReviewSortOption = _reviewSortOptions[ index ];

        await GetReviews( ( ReviewSortType ) _selectedReviewSortOption, _reviewRows, _reviewPage );
    }
    async Task SelectReviewRows( int index )
    {
        if ( index < 0 || index >= _reviewsPerPageOptions.Count )
            return;

        _reviewRows = _reviewsPerPageOptions[ index ];

        await GetReviews( ( ReviewSortType ) _selectedReviewSortOption, _reviewRows, _reviewPage );
        StateHasChanged();
    }
}