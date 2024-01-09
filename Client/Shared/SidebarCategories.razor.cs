using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Shared;

public partial class SidebarCategories : RazorView
{
    [Inject] ICategoryServiceClient CategoryService { get; init; } = default!;

    CategoryDataDto _categoryDataDto = new();
    List<int> currentCategoryIds = new();
    int? currentParentId;
    
    protected override async Task OnInitializedAsync()
    {
        ServiceReply<CategoryDataDto?> reply = await CategoryService.GetCategories();

        if ( !reply.Success || reply.Payload is null )
            return;

        _categoryDataDto = reply.Payload;
        currentCategoryIds = _categoryDataDto.PrimaryIds;
    }

    void SelectCategory( int categoryId )
    {
        CategoryFullDto categoryFull = _categoryDataDto.CategoriesById[ categoryId ];
        currentCategoryIds = categoryFull.Children.Select( c => c.CategoryId ).ToList();
        currentParentId = categoryFull.CategoryId;
    }
    void GoBack()
    {
        if ( !currentParentId.HasValue ) 
            return;

        CategoryFullDto parentCategoryFull = _categoryDataDto.CategoriesById[ currentParentId.Value ];
        currentParentId = parentCategoryFull.ParentCategoryId;
        currentCategoryIds = currentParentId.HasValue
            ? _categoryDataDto.CategoriesById[ currentParentId.Value ].Children.Select( c => c.CategoryId ).ToList()
            : _categoryDataDto.PrimaryIds;
    }
}