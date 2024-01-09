using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Client.Services.Products;
using BlazorElectronics.Client.Services.Specs;
using BlazorElectronics.Client.Services.Vendors;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Products.Search;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorElectronics.Client.Pages.Products;

public partial class ProductSearch : PageView, IDisposable
{
    [Inject] IProductServiceClient ProductService { get; set; } = default!;
    [Inject] ICategoryServiceClient CategoryService { get; init; } = default!;
    [Inject] ISpecServiceClient SpecService { get; init; } = default!;
    [Inject] IVendorServiceClient VendorService { get; init; } = default!;

    [Parameter] public string PrimaryCategory { get; init; } = string.Empty;
    [Parameter] public string SecondaryCategory { get; init; } = string.Empty;
    [Parameter] public string TertiaryCategory { get; init; } = string.Empty;
    
    bool _urlSale;
    bool _urlFeatures;
    bool _urlFeaturedDeals;
    int? _urlVendorId;

    ProductSearchHeader _searchHeader = default!;
    ProductFilters _filters = default!;
    ProductSearchList _searchList = default!;

    CategoryFullDto? _primaryCategory;
    CategoryFullDto? _currentCategory;

    CategoryDataDto categories = new();
    LookupSpecsDto specs = new();
    VendorsDto vendors = new();

    readonly ProductSearchRequestDto _searchRequest = new();
    ProductSearchReplyDto? _searchResults;

    public void Dispose()
    {
        NavManager.LocationChanged -= HandleLocationChanged;
    }
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        NavManager.LocationChanged += HandleLocationChanged;
    }
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
            await LoadPage();
    }
    async Task<bool> GetMetaData()
    {
        List<Task> apiTasks = new();
        
        Task<ServiceReply<CategoryDataDto?>> categoryTask = CategoryService.GetCategories();
        Task<ServiceReply<LookupSpecsDto?>> specTask = SpecService.GetSpecLookups();
        Task<ServiceReply<VendorsDto?>> vendorTask = VendorService.GetVendors();

        apiTasks.Add( categoryTask );
        apiTasks.Add( specTask );
        apiTasks.Add( vendorTask );

        await Task.WhenAll( apiTasks );

        ServiceReply<CategoryDataDto?> categoryReply = categoryTask.Result;
        ServiceReply<LookupSpecsDto?> specReply = specTask.Result;
        ServiceReply<VendorsDto?> vendorReply = vendorTask.Result;

        if ( categoryReply is not { Success: true, Payload: not null } )
        {
            SetViewMessage( false, "Failed to load product categories! " + categoryReply.ErrorType + categoryReply.Message );
            return false;
        }
        
        categories = categoryReply.Payload;

        if ( specReply.Payload is not null )
            specs = specReply.Payload;

        if ( vendorReply.Payload is not null )
            vendors = vendorReply.Payload;

        return true;
    }
    async Task LoadPage()
    {
        PageIsLoaded = false;

        if ( !await GetMetaData() )
            return;

        if ( !ParseUrl( categories ) )
            return;

        HandleUrlSearchParams();
        
        _searchHeader.SetBreadcrumbUrls( _currentCategory, categories.CategoriesById );
        _filters.InitializeFilters( _searchRequest.SearchText, GetCategoryFilters(), GetSpecFilters(), GetVendorFilters() );

        await SearchProducts();
        PageIsLoaded = true;
    }
    bool ParseUrl( CategoryDataDto cDataDto )
    {
        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        Dictionary<string, string> queryParams = ParseQuery( uri.Query );
        queryParams.TryGetValue( Routes.PARAM_SEARCH_TEXT, out string? searchText );
        queryParams.TryGetValue( Routes.PARAM_SEARCH_SALES, out string? sale );
        queryParams.TryGetValue( Routes.PARAM_SEARCH_FEATURED, out string? featured );
        queryParams.TryGetValue( Routes.PARAM_SEARCH_FEATURED_DEALS, out string? featuredDeals );
        queryParams.TryGetValue( Routes.PARAM_SEARCH_VENDOR, out string? vendor );

        _searchRequest.SearchText = searchText;

        if ( !string.IsNullOrWhiteSpace( sale ) )
            _urlSale = true;
        if ( !string.IsNullOrWhiteSpace( featured ) )
            _urlFeatures = true;
        if ( !string.IsNullOrWhiteSpace( featuredDeals ) )
            _urlFeaturedDeals = true;

        if ( int.TryParse( vendor, out int id ) )
            _urlVendorId = id;

        if ( string.IsNullOrWhiteSpace( PrimaryCategory ) )
            return true;

        string categoryUrl = PrimaryCategory;

        if ( !string.IsNullOrWhiteSpace( SecondaryCategory ) )
            categoryUrl = $"{categoryUrl}/{SecondaryCategory}";
        if ( !string.IsNullOrWhiteSpace( TertiaryCategory ) )
            categoryUrl = $"{categoryUrl}/{TertiaryCategory}";
        
        if ( !cDataDto.Urls.TryGetValue( categoryUrl, out int categoryId ) )
            return false;
        if ( !cDataDto.CategoriesById.TryGetValue( categoryId, out CategoryFullDto? category ) )
            return false;

        _currentCategory = category;
        _primaryCategory = _currentCategory;
        _searchRequest.CategoryId = _primaryCategory?.CategoryId;

        if ( _primaryCategory is null )
            return true;
        
        while ( _primaryCategory.Tier != CategoryTier.Primary )
        {
            if ( _primaryCategory.ParentCategoryId is null )
                return false;
            
            if ( !cDataDto.CategoriesById.TryGetValue( _primaryCategory.ParentCategoryId.Value, out _primaryCategory ) )
                return false;
        }

        return _primaryCategory is not null;
    }
    void HandleUrlSearchParams()
    {
        if ( _urlSale )
        {
            _filters.SetSale();
            _searchRequest.Filters.OnSale = true;
        }

        if ( _urlFeatures )
        {
            _filters.SetFeatured();
            _searchRequest.Filters.Featured = true;
        }

        if ( _urlFeaturedDeals )
        {
            _filters.SetFeaturedDeals();
            _searchRequest.Filters.OnSale = true;
            _searchRequest.Filters.Featured = true;
        }

        if ( _urlVendorId is not null )
        {
            _filters.SetVendor( _urlVendorId.Value );
            _searchRequest.Filters.Vendors.Add( _urlVendorId.Value );
        }
    }
    List<CategoryFullDto> GetCategoryFilters()
    {
        if ( _currentCategory is null )
            return categories.GetPrimaryCategories();

        return _currentCategory.Children.Count > 0
            ? _currentCategory.Children
            : new List<CategoryFullDto>();
    }
    Dictionary<int, LookupSpec> GetSpecFilters()
    {
        if ( specs is null )
            return new Dictionary<int, LookupSpec>();

        Dictionary<int, LookupSpec> specFilters = specs.GlobalSpecIds
            .ToDictionary( id => id, id => specs.SpecsById[ id ] );

        if ( _primaryCategory is null || !specs.SpecIdsByCategory.TryGetValue( _primaryCategory.CategoryId, out List<int>? specsCategory ) )
            return specFilters;
        
        foreach ( int id in specsCategory )
        {
            specFilters.Add( id, specs.SpecsById[ id ] );
        }

        return specFilters;
    }
    List<VendorDto> GetVendorFilters()
    {
        if ( vendors is null )
            return new List<VendorDto>();

        if ( _primaryCategory is null )
            return vendors.VendorsById.Values.ToList();

        if ( !vendors.VendorIdsByCategory.TryGetValue( _primaryCategory.CategoryId, out List<int>? vendorIds ) )
            return new List<VendorDto>();

        List<VendorDto> vendorsCategory = new();

        foreach ( int id in vendorIds )
        {
            if ( vendors.VendorsById.TryGetValue( id, out VendorDto? m ) )
                vendorsCategory.Add( m );
        }

        return vendorsCategory;
    }
    
    void HandleLocationChanged( object? obj, LocationChangedEventArgs args )
    {
        NavManager.NavigateTo( NavManager.Uri, true );
    }
    void OpenFilters()
    {
        _filters.ShowFilters();
    }
    
    async Task ApplyFilters()
    {
        _searchList.SetPage( 1 );
        _searchRequest.Page = 1;
        await SearchProducts();
    }
    async Task ApplySort( ProductSortType type )
    {
        _searchList.SetPage( 1 );
        _searchRequest.Page = 1;
        _searchRequest.SortType = type;
        await SearchProducts();
    }
    async Task ApplyRows( int rows )
    {
        _searchList.SetPage( 1 );
        _searchRequest.Rows = rows;
        _searchRequest.Page = 1;
        await SearchProducts();
    }
    async Task ApplyPage( int page )
    {
        _searchRequest.Page = page;
        _searchList.SetPage( page );
        _searchHeader.SetPage( page );
        await SearchProducts();
    }
    async Task SearchProducts()
    {
        ServiceReply<ProductSearchReplyDto?> reply = await ProductService.GetProductSearch( _searchRequest );

        if ( !reply.Success || reply.Payload is null )
        {
            InvokeAlert( AlertType.Danger, reply.ErrorType + " : " + reply.Message );
            return;
        }
        
        _searchResults = reply.Payload;
        _searchHeader.OnSearchResults( _searchRequest.Page, _searchResults );
        _searchList.OnSearch( _searchRequest.Rows, _searchResults, categories.CategoriesById, vendors.VendorsById );
        StateHasChanged();
    }
}