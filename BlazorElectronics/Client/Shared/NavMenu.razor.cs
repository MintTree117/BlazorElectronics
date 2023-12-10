using BlazorElectronics.Client.Models;
using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Shared;

public partial class NavMenu : RazorView
{
    public event Action<CategoriesResponse?>? OnCategoriesLoaded;
    public event Action<SessionMeta?>? OnSessionLoaded;
    public event Action? OnToggleSidebar;
    
    [Inject] public ICategoryServiceClient CategoryService { get; set; } = default!;
    [Inject] public IUserServiceClient UserService { get; set; } = default!;

    bool _loadedCategories = false;
    string _categoryMessage = "Loading Categories...";
    
    CategoriesResponse? _categories;
    
    public const string HREF_HOME = "";

    public void NavigateTo( string url )
    {
        
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Task<ServiceReply<CategoriesResponse?>> categoryTask = CategoryService.GetCategories();
        Task<SessionMeta?> userTask = UserService.GetSessionMeta();

        List<Task> tasks = new();
        
        tasks.Add( categoryTask );
        tasks.Add( userTask );

        await Task.WhenAll( tasks );

        _categories = categoryTask.Result.Data;

        OnCategoriesLoaded?.Invoke( _categories );
        OnSessionLoaded?.Invoke( userTask.Result );
    }

    void ToggleSidebar()
    {
        OnToggleSidebar?.Invoke();
    }
    void GoToHome()
    {
        NavManager.NavigateTo( "" );
    }
}