using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Shared;

public partial class NavSidebar : RazorView, IDisposable
{
    [Parameter] public NavMenu NavMenu { get; set; } = default!;
    
    bool _collapseNavMenu = true;
    string? _navMenuCssClass => _collapseNavMenu ? "nav-sidebar-hide" : "";
    bool _isAuthorized = false;
    bool _isAdmin = false;

    SessionMeta? _sessionMeta;
    CategoriesResponse _categories = new();
    List<int> currentCategoryIds = new();
    int? currentParentId;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        NavMenu.OnCategoriesLoaded += SetCategories;
        NavMenu.OnSessionLoaded += SetAuthorization;
        NavMenu.OnToggleSidebar += ToggleSidebar;
    }

    void SetCategories( CategoriesResponse? r )
    {
        _categories = r;
        currentCategoryIds = r.PrimaryIds;
        StateHasChanged();
    }
    void SetAuthorization( SessionMeta? session )
    {
        _sessionMeta = session;

        if ( _sessionMeta is null )
        {
            _isAuthorized = false;
            _isAdmin = false;
            StateHasChanged();
            return;
        }
        
        _isAuthorized = _sessionMeta is not null && _sessionMeta.Type != SessionType.None;
        _isAdmin = _sessionMeta.Type == SessionType.Admin;
        StateHasChanged();
    }
    void ToggleSidebar()
    {
        _collapseNavMenu = !_collapseNavMenu;
        StateHasChanged();
    }
    
    public void Dispose()
    {
        NavMenu.OnToggleSidebar -= ToggleSidebar;
        NavMenu.OnSessionLoaded -= SetAuthorization;
        NavMenu.OnCategoriesLoaded -= SetCategories;
    }

    void SelectCategory( int categoryId )
    {
        CategoryModel category = _categories.CategoriesById[ categoryId ];
        currentCategoryIds = category.Children.Select( c => c.CategoryId ).ToList();
        currentParentId = category.CategoryId;
    }
    void GoBack()
    {
        if ( !currentParentId.HasValue )
            return;

        CategoryModel parentCategory = _categories.CategoriesById[ currentParentId.Value ];
        currentParentId = parentCategory.ParentCategoryId;
        currentCategoryIds = currentParentId.HasValue
            ? _categories.CategoriesById[ currentParentId.Value ].Children.Select( c => c.CategoryId ).ToList()
            : _categories.PrimaryIds;
    }
}