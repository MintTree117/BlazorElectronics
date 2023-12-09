using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Client.Services.Specs;
using BlazorElectronics.Client.Services.Vendors;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.ProductSearch;

public partial class ProductSearch : PageView
{
    public event Action<Dictionary<string,string>>? OnSetBreadcrumb; 
    public event Action<List<CategoryModel>?, Dictionary<int, Spec>, List<VendorModel>>? OnSetFilters;

    [Inject] ICategoryServiceClient CategoryService { get; init; } = default!;
    [Inject] ISpecServiceClient SpecService { get; init; } = default!;
    [Inject] IVendorServiceClient VendorService { get; init; } = default!;

    [Parameter] public string PrimaryCategory { get; init; } = string.Empty;
    [Parameter] public string SecondaryCategory { get; init; } = string.Empty;
    [Parameter] public string TertiaryCategory { get; init; } = string.Empty;

    bool _hasCategory;
    int _primaryCategoryId;
    int _currentCategoryId;

    CategoriesResponse categories = new();
    SpecsResponse? specs;
    VendorsResponse? vendors;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !await GetData() )
            return;

        if ( !ParseUrl() )
            return;

        OnSetBreadcrumb?.Invoke( GetBreadcrumbNavigation() );
        OnSetFilters?.Invoke( GetSubcategories(), GetSpecFilters(), GetVendorFilters() );

        PageIsLoaded = true;
    }
    
    async Task<bool> GetData()
    {
        List<Task> apiTasks = new();
        
        Task<ServiceReply<CategoriesResponse?>> categoryTask = CategoryService.GetCategories();
        Task<ServiceReply<SpecsResponse?>> specTask = SpecService.GetSpecLookups();
        Task<ServiceReply<VendorsResponse?>> vendorTask = VendorService.GetVendors();

        apiTasks.Add( categoryTask );
        apiTasks.Add( specTask );
        apiTasks.Add( vendorTask );

        await Task.WhenAll( apiTasks );

        ServiceReply<CategoriesResponse?> categoryReply = categoryTask.Result;
        ServiceReply<SpecsResponse?> specReply = specTask.Result;
        ServiceReply<VendorsResponse?> vendorReply = vendorTask.Result;

        if ( categoryReply is not { Success: true, Data: not null } )
        {
            SetActionMessage( false, "Failed to load product categories! " + categoryReply.ErrorType + categoryReply.Message );
            return false;   
        }
        
        categories = categoryReply.Data;
        specs = specReply.Data;
        vendors = vendorReply.Data;

        return true;
    }
    bool ParseUrl()
    {
        if ( string.IsNullOrWhiteSpace( PrimaryCategory ) )
            return true;

        if ( !categories.Urls.TryGetValue( PrimaryCategory, out _primaryCategoryId ) )
        {
            SetActionMessage( false, "Invalid url!" );
            return false;
        }
        
        _hasCategory = true;
        _currentCategoryId = _primaryCategoryId;

        if ( string.IsNullOrWhiteSpace( SecondaryCategory ) )
            return true;

        if ( !categories.Urls.TryGetValue( SecondaryCategory, out int secondaryId ) )
        {
            SetActionMessage( false, "Invalid url!" );
            return false;
        }

        _currentCategoryId = secondaryId;

        if ( string.IsNullOrWhiteSpace( TertiaryCategory ) )
            return true;

        if ( !categories.Urls.TryGetValue( TertiaryCategory, out int tertiaryId ) )
        {
            SetActionMessage( false, "Invalid url!" );
            return false;
        }

        _currentCategoryId = tertiaryId;

        return true;
    }
    Dictionary<string, string> GetBreadcrumbNavigation()
    {
        Dictionary<string, string> urls = new() { { "Search", "/search" } };

        if ( !_hasCategory )
            return urls;

        if ( !categories.Urls.TryGetValue( PrimaryCategory, out int id ) || !categories.CategoriesById.TryGetValue( id, out CategoryModel? m ) )
            return urls;
        
        urls.Add( m.Name, $"/search/{PrimaryCategory}" );

        if ( !categories.Urls.TryGetValue( PrimaryCategory, out id ) || !categories.CategoriesById.TryGetValue( id, out m ) )
            return urls;

        urls.Add( m.Name, $"/search/{PrimaryCategory}/{SecondaryCategory}" );

        if ( !categories.Urls.TryGetValue( PrimaryCategory, out id ) || !categories.CategoriesById.TryGetValue( id, out m ) )
            return urls;

        urls.Add( m.Name, $"/search/{PrimaryCategory}/{SecondaryCategory}/{TertiaryCategory}" );

        return urls;
    }
    List<CategoryModel> GetSubcategories()
    {
        return categories.CategoriesById.TryGetValue( _currentCategoryId, out CategoryModel? category )
            ? category.Children
            : new List<CategoryModel>();
    }
    Dictionary<int, Spec> GetSpecFilters()
    {
        if ( specs is null )
            return new Dictionary<int, Spec>();
        
        Dictionary<int, Spec> specFilters = specs.GlobalSpecIds
            .ToDictionary( id => id, id => specs.SpecsById[ id ] );

        if ( !_hasCategory ) 
            return specFilters;
        
        foreach ( int id in specs.SpecIdsByCategory[ _primaryCategoryId ] )
        {
            specFilters.Add( id, specs.SpecsById[ id ] );
        }

        return specFilters;
    }
    List<VendorModel> GetVendorFilters()
    {
        if ( vendors is null )
            return new List<VendorModel>();

        if ( !_hasCategory )
            return vendors.VendorsById.Values.ToList();

        if ( !vendors.VendorIdsByCategory.TryGetValue( _primaryCategoryId, out List<int>? vendorIds ) )
            return new List<VendorModel>();

        List<VendorModel> vendorsCategory = new();

        foreach ( int id in vendorIds )
        {
            if ( vendors.VendorsById.TryGetValue( id, out VendorModel? m ) )
                vendorsCategory.Add( m );
        }

        return vendorsCategory;
    }

    public void ApplyFilters( ProductFilters filters )
    {
        
    }
    public void ApplySort()
    {
        
    }
    public void NavigateBack( string url )
    {
        NavManager.NavigateTo( url, true );
    }
    public void NavigateForward( CategoryModel category )
    {
        NavManager.NavigateTo( $"{NavManager.Uri}/{category.ApiUrl}", true );
    }
}