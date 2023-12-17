using System.Text;
using System.Text.RegularExpressions;
using BlazorElectronics.Client.Models;
using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin.BulkInsert;

public partial class BulkCategories : AdminPage
{
    [Inject] public IAdminBulkServiceClient BulkService { get; set; } = default!;
    [Inject] public IAdminCategoryHelper CategoryHelper { get; set; } = default!;
    CategorySeed _seed = new();
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        ServiceReply<bool> categoryResponse = await CategoryHelper.Init();

        if ( !categoryResponse.Success )
        {
            SetViewMessage( false, categoryResponse.Message ?? IAdminCategoryHelper.DEFAULT_ERROR_MESSAGE );
            return;
        }

        if ( CategoryHelper.Categories.All( c => c.Tier != CategoryTier.Primary ) )
        {
            InvokeAlert( AlertType.Danger, "There are no Primary Categories!" );
            return;
        }

        PageIsLoaded = true;
        NewBulk();
    }

    void NewBulk()
    {
        _seed = new CategorySeed
        {
            Tier = CategoryTier.Secondary,
            ParentCategoryId = 0,
            PrimaryId = 1
        };
        StateHasChanged();
    }
    List<CategoryViewDtoDto> GetPrimary()
    {
        return CategoryHelper.Categories.Where( c => c.Tier == CategoryTier.Primary ).ToList();
    }
    List<CategoryViewDtoDto> GetEditParents()
    {
        return _seed.Tier == CategoryTier.Tertiary
            ? CategoryHelper.Categories.Where( c => c.ParentCategoryId == _seed.PrimaryId ).ToList()
            : CategoryHelper.Categories.Where( c => c.Tier == CategoryTier.Primary ).ToList();
    }
    async Task Submit()
    {
        List<string> names = _seed.Names
            .Split( "," )
            .ToList();

        List<CategoryEditDtoDto> categories = names
            .Select( s => new CategoryEditDtoDto
            {
                ParentCategoryId = _seed.ParentCategoryId,
                Tier = _seed.Tier,
                Name = s,
                ApiUrl = ConvertToUrlFriendly( s ),
                ImageUrl = "default"
            } )
            .ToList();

        ServiceReply<bool> reply = await BulkService.BulkInsertCategories( categories );

        if ( !reply.Success )
        {
            Logger.LogError( reply.ErrorType + reply.Message );
            InvokeAlert( AlertType.Danger, reply.ErrorType + reply.Message );
            return;
        }

        InvokeAlert( AlertType.Success, "Successfully inserted bulk categories." );
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