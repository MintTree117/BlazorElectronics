using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Shared;

public partial class SidebarCategories : RazorView
{
    [Inject] ICategoryServiceClient CategoryService { get; init; } = default!;

    CategoryData _categoryData = new();
    List<int> currentCategoryIds = new();
    int? currentParentId;
    
    protected override async Task OnInitializedAsync()
    {
        ServiceReply<CategoryData?> reply = await CategoryService.GetCategories();

        if ( !reply.Success || reply.Data is null )
            return;

        _categoryData = reply.Data;
        currentCategoryIds = _categoryData.PrimaryIds;
    }

    void SelectCategory( int categoryId )
    {
        CategoryModel category = _categoryData.CategoriesById[ categoryId ];
        currentCategoryIds = category.Children.Select( c => c.CategoryId ).ToList();
        currentParentId = category.CategoryId;
    }
    void GoBack()
    {
        if ( !currentParentId.HasValue ) 
            return;

        CategoryModel parentCategory = _categoryData.CategoriesById[ currentParentId.Value ];
        currentParentId = parentCategory.ParentCategoryId;
        currentCategoryIds = currentParentId.HasValue
            ? _categoryData.CategoriesById[ currentParentId.Value ].Children.Select( c => c.CategoryId ).ToList()
            : _categoryData.PrimaryIds;
    }
}