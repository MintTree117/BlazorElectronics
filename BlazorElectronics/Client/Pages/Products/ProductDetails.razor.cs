using System.Text.RegularExpressions;
using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Client.Services.Products;
using BlazorElectronics.Client.Services.Reviews;
using BlazorElectronics.Client.Services.Specs;
using BlazorElectronics.Client.Services.Vendors;
using BlazorElectronics.Client.Shared;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Reviews;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorElectronics.Client.Pages.Products;

public partial class ProductDetails : PageView
{
    [Inject] public ICategoryServiceClient CategoryService { get; set; } = default!;
    [Inject] public IProductServiceClient ProductService { get; init; } = default!;
    [Inject] public IReviewServiceClient ReviewService { get; set; } = default!;
    [Inject] public ISpecServiceClient LookupService { get; set; } = default!;
    [Inject] public IVendorServiceClient VendorsService { get; set; } = default!;

    [Parameter] public string ProductId { get; set; } = string.Empty;
    
    Pagination _pagination = default!;
    ProductDto? _product;
    
    readonly List<CategoryFullDto> _categories = new();
    readonly Dictionary<string, string> _lookupSpecsAggregated = new();
    readonly VendorDto _vendor = new();
    
    readonly List<ProductReviewDto> _reviews = new();
    int _reviewCount = 0;
    readonly List<string> _reviewSortOptions = GetReviewSortOptions();
    readonly List<int> _reviewsPerPageOptions = new() { 5, 10, 15, 20 };
    readonly ProductReviewsGetDto _productReviewsGetDto = new();

    public override bool PageIsReady()
    {
        return base.PageIsReady() && _product is not null;
    }
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !ParseUrl( out int id ) )
            return;

        if ( !await LoadProduct( id ) )
            return;

        Task[] tasks = { LoadCategories(), LoadLookups(), LoadVendor(), LoadReviews() };
        await Task.WhenAll( tasks );
    }
    bool ParseUrl( out int id )
    {
        if ( int.TryParse( ProductId, out id ) ) 
            return true;
        
        SetViewMessage( false, "Invalid Url!" );
        return false;
    }
    async Task<bool> LoadProduct( int id )
    {
        ServiceReply<ProductDto?> productReply = await ProductService.GetProductDetails( id );

        if ( !productReply.Success || productReply.Data is null )
        {
            SetViewMessage( false, $"Failed to fetch product details! {productReply.ErrorType} : {productReply.Message}" );
            return false;
        }

        _product = productReply.Data;
        PageIsLoaded = true;
        StateHasChanged();
        return true;
    }
    async Task LoadCategories()
    {
        ServiceReply<CategoryData?> categoriesReply = await CategoryService.GetCategories();

        if ( !categoriesReply.Success || categoriesReply.Data is null )
            return;
        
        GetProductCategories( categoriesReply.Data.CategoriesById );
        StateHasChanged();
    }
    async Task LoadLookups()
    {
        ServiceReply<LookupSpecsDto?> lookupsReply = await LookupService.GetSpecLookups();

        if ( !lookupsReply.Success || lookupsReply.Data is null )
            return;

        GetAggregatedLookups( lookupsReply.Data );
        StateHasChanged();
    }
    async Task LoadVendor()
    {
        if ( _product is null )
            return;
        
        ServiceReply<VendorsDto?> vendorReply = await VendorsService.GetVendors();

        if ( !vendorReply.Success || vendorReply.Data is null )
            return;

        if ( !vendorReply.Data.VendorsById.TryGetValue( _product.VendorId, out VendorDto? vendor ) )
            return;

        _vendor.VendorId = vendor.VendorId;
        _vendor.VendorName = vendor.VendorName;
        _vendor.VendorUrl = vendor.VendorUrl;
        StateHasChanged();
    }
    async Task LoadReviews()
    {
        if ( _product is null )
            return;

        _productReviewsGetDto.ProductId = _product.Id;
        _reviews.Clear();

        ServiceReply<ProductReviewsReplyDto?> reply = await ReviewService.GetForProduct( _productReviewsGetDto );

        if ( !reply.Success || reply.Data is null )
            return;

        _reviews.AddRange( reply.Data.Reviews );
        _reviewCount = reply.Data.TotalMatches;
        _pagination.UpdateTotalPages( _productReviewsGetDto.Rows, reply.Data.TotalMatches );
        StateHasChanged();
    }

    static List<string> GetReviewSortOptions()
    {
        return Enum
            .GetNames<ReviewSortType>()
            .Select( name => Regex
                .Replace( name, "(\\B[A-Z])", " $1" ) )
            .ToList();
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
    void GetAggregatedLookups( LookupSpecsDto lookups )
    {
        if ( _product is null )
            return;
        
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

    async Task SelectReviewPage( int page )
    {
        _productReviewsGetDto.Page = page;
        await LoadReviews();
    }
    async Task SelectReviewSort( int index )
    {
        if ( index < 0 || index >= _reviewSortOptions.Count  )
            return;

        ReviewSortType type;

        try
        {
            type = ( ReviewSortType ) index + 1;
        }
        catch
        {
            Logger.LogError( "Invalid Review Sort Type!" );
            return;
        }
        
        _productReviewsGetDto.SortType = type;
        await LoadReviews();
    }
    async Task SelectReviewRows( int index )
    {
        if ( index < 0 || index >= _reviewsPerPageOptions.Count )
            return;

        _productReviewsGetDto.Rows = _reviewsPerPageOptions[ index ];
        await LoadReviews();
    }
}