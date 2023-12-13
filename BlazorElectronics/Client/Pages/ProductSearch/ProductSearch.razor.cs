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

namespace BlazorElectronics.Client.Pages.ProductSearch;

public partial class ProductSearch : PageView
{
    public event Action<Dictionary<string,string>>? InitializeHeader; 
    public event Action<List<CategoryModel>?, Dictionary<int, Spec>, List<VendorModel>>? InitializeFilters;
    public event Action<ProductSearchResponse>? OnProductSearch;

    [Inject] IProductServiceClient ProductService { get; set; } = default!;
    [Inject] ICategoryServiceClient CategoryService { get; init; } = default!;
    [Inject] ISpecServiceClient SpecService { get; init; } = default!;
    [Inject] IVendorServiceClient VendorService { get; init; } = default!;

    [Parameter] public string PrimaryCategory { get; init; } = string.Empty;
    [Parameter] public string SecondaryCategory { get; init; } = string.Empty;
    [Parameter] public string TertiaryCategory { get; init; } = string.Empty;

    CategoryModel? _primaryCategory;
    CategoryModel? _currentCategory;

    CategoryData? categories;
    SpecsResponse? specs;
    VendorsResponse? vendors;
    ProductSearchFilters? _searchFilters;
    ProductSortType _sortType;
    int _currentRows = 10;
    int _currentPage = 1;
    ProductSearchResponse? _searchResults;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !await GetMetaData() )
            return;

        await LoadPage();
    }
    async Task<bool> GetMetaData()
    {
        List<Task> apiTasks = new();
        
        Task<ServiceReply<CategoryData?>> categoryTask = CategoryService.GetCategories();
        Task<ServiceReply<SpecsResponse?>> specTask = SpecService.GetSpecLookups();
        Task<ServiceReply<VendorsResponse?>> vendorTask = VendorService.GetVendors();

        apiTasks.Add( categoryTask );
        apiTasks.Add( specTask );
        apiTasks.Add( vendorTask );

        await Task.WhenAll( apiTasks );

        ServiceReply<CategoryData?> categoryReply = categoryTask.Result;
        ServiceReply<SpecsResponse?> specReply = specTask.Result;
        ServiceReply<VendorsResponse?> vendorReply = vendorTask.Result;

        if ( categoryReply is not { Success: true, Data: not null } )
        {
            InvokeAlert( AlertType.Danger, "Failed to load product categories! " + categoryReply.ErrorType + categoryReply.Message );
            return false;   
        }
        
        categories = categoryReply.Data;
        specs = specReply.Data;
        vendors = vendorReply.Data;

        return true;
    }
    async Task LoadPage()
    {
        PageIsLoaded = false;

        if ( !ParseUrl( categories! ) )
            return;

        InitializeHeader?.Invoke( GetBreadcrumbNavigation() );
        InitializeFilters?.Invoke( _currentCategory?.Children, GetSpecFilters(), GetVendorFilters() );

        await SearchProducts();

        PageIsLoaded = true;
    }
    bool ParseUrl( CategoryData cData )
    {
        if ( string.IsNullOrWhiteSpace( PrimaryCategory ) )
            return true;

        string categoryUrl = PrimaryCategory;

        if ( !string.IsNullOrWhiteSpace( SecondaryCategory ) )
            categoryUrl = $"{categoryUrl}/{SecondaryCategory}";
        if ( !string.IsNullOrWhiteSpace( TertiaryCategory ) )
            categoryUrl = $"{categoryUrl}/{TertiaryCategory}";
        
        if ( !cData.Urls.TryGetValue( categoryUrl, out int categoryId ) )
            return false;
        if ( !cData.CategoriesById.TryGetValue( categoryId, out CategoryModel? category ) )
            return false;

        _currentCategory = category;
        _primaryCategory = _currentCategory;
        
        while ( _primaryCategory.Tier != CategoryTier.Primary )
        {
            if ( _primaryCategory.ParentCategoryId is null )
                return false;
            
            if ( !cData.CategoriesById.TryGetValue( _primaryCategory.ParentCategoryId.Value, out _primaryCategory ) )
                return false;
        }

        return _primaryCategory is not null;
    }
    Dictionary<string, string> GetBreadcrumbNavigation()
    {
        Dictionary<string, string> urls = new() { { "Search", Routes.SEARCH } };
        
        if ( categories is null )
            return urls;
        
        CategoryModel? m = _currentCategory;

        while ( m is not null )
        {
            if ( m.ParentCategoryId is null || 
                 !categories.CategoriesById.TryGetValue( m.ParentCategoryId.Value, out m ) )
                break;

            urls.Add( m.Name, $"{Routes.SEARCH}/{m.ApiUrl}" );
        }

        return urls;
    }
    Dictionary<int, Spec> GetSpecFilters()
    {
        if ( specs is null )
            return new Dictionary<int, Spec>();

        Dictionary<int, Spec> specFilters = specs.GlobalSpecIds
            .ToDictionary( id => id, id => specs.SpecsById[ id ] );

        if ( _primaryCategory is null || !specs.SpecIdsByCategory.TryGetValue( _primaryCategory.CategoryId, out List<int>? specsCategory ) )
            return specFilters;
        
        foreach ( int id in specsCategory )
        {
            specFilters.Add( id, specs.SpecsById[ id ] );
        }

        return specFilters;
    }
    List<VendorModel> GetVendorFilters()
    {
        if ( vendors is null )
            return new List<VendorModel>();

        if ( _primaryCategory is null )
            return vendors.VendorsById.Values.ToList();

        if ( !vendors.VendorIdsByCategory.TryGetValue( _primaryCategory.CategoryId, out List<int>? vendorIds ) )
            return new List<VendorModel>();

        List<VendorModel> vendorsCategory = new();

        foreach ( int id in vendorIds )
        {
            if ( vendors.VendorsById.TryGetValue( id, out VendorModel? m ) )
                vendorsCategory.Add( m );
        }

        return vendorsCategory;
    }

    public async Task ApplyFilters( ProductSearchFilters filters )
    {
        _searchFilters = filters;

        await SearchProducts();
    }
    public async Task ApplySort( ProductSortType type )
    {
        _sortType = type;

        await SearchProducts();
    }
    public async Task ApplyRows( int rows )
    {
        _currentRows = rows;

        await SearchProducts();
    }
    async Task ApplyPagination( int page )
    {
        _currentPage = page;

        await SearchProducts();
    }
    async Task SearchProducts()
    {
        ProductSearchRequest request = new()
        {
            CategoryId = _currentCategory?.CategoryId,
            SortType = _sortType,
            Filters = _searchFilters,
            Rows = _currentRows,
            Page = _currentPage
        };
        
        Logger.LogError( request.CategoryId.ToString() );
        
        ServiceReply<ProductSearchResponse?> reply = await ProductService.GetProductSearch( request );

        if ( !reply.Success || reply.Data is null )
        {
            InvokeAlert( AlertType.Danger, reply.ErrorType + " : " + reply.Message );
            return;
        }

        _searchResults = reply.Data;
        OnProductSearch?.Invoke( _searchResults );
        StateHasChanged();
    }
}