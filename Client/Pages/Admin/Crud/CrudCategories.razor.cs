using System.Text;
using System.Text.RegularExpressions;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Client.Pages.Admin.Crud;

public sealed partial class CrudCategories : CrudPage<CategoryViewDtoDto, CategoryEditDto>
{
    protected override async Task OnInitializedAsync()
    {
        ItemTitle = "Category";
        
        await base.OnInitializedAsync();
        
        if ( !PageIsAuthorized )
            return;
        
        ApiPath = "api/AdminCategory";
        await LoadView();
    }

    protected override void GenerateTableMeta()
    {
        base.GenerateTableMeta();
        THeadMeta.Add( "Category Tier", SortByTier );
        THeadMeta.Add( "Category Parent", SortByParent );
    }
    protected override string GetEditTitle()
    {
        return $"Edit {ItemEdit.Tier} {ItemTitle}";
    }

    protected override async Task SubmitNew()
    {
        if ( string.IsNullOrWhiteSpace( ItemEdit.ApiUrl ) )
        {
            ItemEdit.ApiUrl = ConvertToUrlFriendly( ItemEdit.Name );
        }
        
        await base.SubmitNew();
    }
    public override void CreateItem()
    {
        base.CreateItem();
        ItemEdit.Tier = CategoryTier.Primary;
        ItemEdit.PrimaryId = 1;
        ItemEdit.ParentCategoryId = null;
    }
    List<CategoryViewDtoDto> GetPrimary()
    {
        return ItemsView.Where( c => c.Tier == CategoryTier.Primary ).ToList();
    }
    List<CategoryViewDtoDto> GetEditParents()
    {
        return ItemEdit.Tier == CategoryTier.Tertiary 
            ? ItemsView.Where( c => c.ParentCategoryId == ItemEdit.PrimaryId ).ToList() 
            : ItemsView.Where( c => c.Tier == CategoryTier.Primary ).ToList();
    }
    void SortByTier()
    {
        ItemsView = ItemsView.OrderBy( c => c.Tier ).ToList();
    }
    void SortByParent()
    {
        ItemsView = ItemsView.OrderBy( c => c.ParentCategoryId ).ToList();
    }

    static string ConvertToUrlFriendly( string text )
    {
        // Normalize Unicode characters
        //text = text.Normalize( NormalizationForm.FormKD );

        // Remove or replace special characters
        StringBuilder sb = new();
        foreach ( char c in text )
        {
            if ( char.IsLetterOrDigit( c ) || c == '-' )
            {
                sb.Append( c );
            }
            else if ( char.IsWhiteSpace( c ) )
            {
                sb.Append( '-' );
            }
        }

        string urlFriendly = sb.ToString();

        // Convert to lowercase
        urlFriendly = urlFriendly.ToLower();

        // Trim extra hyphens
        urlFriendly = Regex.Replace( urlFriendly, "-+", "-" ).Trim( '-' );

        return urlFriendly;
    }
}