using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin.Categories;

public sealed partial class AdminCategoriesView : AdminView<CategoryViewDto>
{
    [Inject] IAdminCategoryServiceClient AdminCategoryService { get; init; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        UrlItemName = "categories";
        ViewService = AdminCategoryService;

        await LoadView();
    }

    protected override List<THeadDisplayMeta> GenerateTHeadMeta()
    {
        List<THeadDisplayMeta> list = base.GenerateTHeadMeta();
        list.Add( new THeadDisplayMeta( "Category Tier", SortByTier ) );
        return list;
    }

    void SortByTier()
    {
        Items = Items.OrderBy( c => c.Tier ).ToList();
    }
}