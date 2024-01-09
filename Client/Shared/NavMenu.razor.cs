using BlazorElectronics.Client.Models;
using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Shared;

public partial class NavMenu : RazorView
{
    public event Action<CategoryDataDto?>? OnCategoriesLoaded;
    public event Action<SessionMeta?>? OnSessionLoaded;
    public event Action<bool>? OnToggleSidebar;
    
    [Inject] public ICategoryServiceClient CategoryService { get; set; } = default!;
    [Inject] public IUserServiceClient UserService { get; set; } = default!;

    CategoryDataDto? _categories;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Task<ServiceReply<CategoryDataDto?>> categoryTask = CategoryService.GetCategories();
        Task<SessionMeta?> userTask = UserService.GetSessionMeta();

        List<Task> tasks = new();
        
        tasks.Add( categoryTask );
        tasks.Add( userTask );

        await Task.WhenAll( tasks );

        _categories = categoryTask.Result.Payload;

        OnCategoriesLoaded?.Invoke( _categories );
        OnSessionLoaded?.Invoke( userTask.Result );
    }

    void OpenSidebar()
    {
        OnToggleSidebar?.Invoke( true );
    }
    void GoToHome()
    {
        NavManager.NavigateTo( "" );
    }
}