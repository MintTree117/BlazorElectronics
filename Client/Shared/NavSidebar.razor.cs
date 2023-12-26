using System.Security;
using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Shared;

public partial class NavSidebar : RazorView, IDisposable
{
    [Parameter] public NavMenu NavMenu { get; set; } = default!;

    bool _show = false;
    bool _isAuthorized = false;
    bool _isAdmin = false;

    SessionMeta? _sessionMeta;
    CategoryData _categories = new();
    List<CategoryFullDto> _primaryCategories = new();
    int? _currentParentId = null;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        NavMenu.OnCategoriesLoaded += SetCategories;
        NavMenu.OnSessionLoaded += SetAuthorization;
        NavMenu.OnToggleSidebar += ToggleSidebar;
    }

    void SetCategories( CategoryData? r )
    {
        if ( r is null )
            return;
        
        _categories = r;
        _primaryCategories = r.GetPrimaryCategories();
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

        if ( _sessionMeta is null )
        {
            _isAuthorized = false;
            _isAdmin = false;
            StateHasChanged();
            return;
        }
        
        _isAuthorized = _sessionMeta.Type != SessionType.None;
        _isAdmin = _sessionMeta.Type == SessionType.Admin;
        StateHasChanged();
    }
    void ToggleSidebar( bool show )
    {
        _show = show;
        StateHasChanged();
    }

    public void Dispose()
    {
        NavMenu.OnToggleSidebar -= ToggleSidebar;
        NavMenu.OnSessionLoaded -= SetAuthorization;
        NavMenu.OnCategoriesLoaded -= SetCategories;
    }

    List<CategoryFullDto> GetCurrentCategories()
    {
        return _currentParentId is not null
            ? _categories.CategoriesById[ _currentParentId.Value ].Children
            : _primaryCategories;
    }
    void NextCategory( CategoryFullDto c )
    {
        _currentParentId = c.CategoryId;
        StateHasChanged();
    }
    void PreviousCategory()
    {
        if ( _currentParentId is null )
            return;
        
        CategoryFullDto parentCategoryFull = _categories.CategoriesById[ _currentParentId.Value ];
        _currentParentId = parentCategoryFull.ParentCategoryId;

        StateHasChanged();
    }
}