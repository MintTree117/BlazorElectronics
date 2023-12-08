using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Shared;

public partial class SidebarCategories : RazorView
{
    [Inject] ICategoryServiceClient CategoryService { get; init; } = default!;

    CategoriesResponse categoriesResponse = new();
    List<int> currentCategoryIds = new();
    int? currentParentId;
    
    protected override async Task OnInitializedAsync()
    {
        ServiceReply<CategoriesResponse?> reply = await CategoryService.GetCategories();

        if ( !reply.Success || reply.Data is null )
            return;

        categoriesResponse = reply.Data;
        currentCategoryIds = categoriesResponse.PrimaryIds;
    }

    void SelectCategory( int categoryId )
    {
        CategoryModel category = categoriesResponse.CategoriesById[ categoryId ];
        currentCategoryIds = category.Children.Select( c => c.CategoryId ).ToList();
        currentParentId = category.CategoryId;
    }
    void GoBack()
    {
        if ( !currentParentId.HasValue ) 
            return;

        CategoryModel parentCategory = categoriesResponse.CategoriesById[ currentParentId.Value ];
        currentParentId = parentCategory.ParentCategoryId;
        currentCategoryIds = currentParentId.HasValue
            ? categoriesResponse.CategoriesById[ currentParentId.Value ].Children.Select( c => c.CategoryId ).ToList()
            : categoriesResponse.PrimaryIds;
    }
}