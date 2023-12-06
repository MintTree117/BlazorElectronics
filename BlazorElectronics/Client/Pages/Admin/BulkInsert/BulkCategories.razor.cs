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
    readonly CategorySeed _seed = new();
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        ServiceReply<bool> categoryResponse = await CategoryHelper.Init();

        if ( !categoryResponse.Success )
        {
            SetActionMessage( false, categoryResponse.Message ?? IAdminCategoryHelper.DEFAULT_ERROR_MESSAGE );
            return;
        }
        
        PageIsLoaded = true;
    }

    void HandleTierChange( ChangeEventArgs e )
    {
        StateHasChanged();
    }
    List<CategoryView> GetEditParents()
    {
        return Enum.TryParse( ( ( int ) _seed.Tier - 1 ).ToString(), out CategoryTier parentTier )
            ? CategoryHelper.Categories.Where( item => item.Tier == parentTier ).ToList()
            : new List<CategoryView>();
    }
    async Task Submit()
    {
        List<string> names = _seed.Names
            .Split( "," )
            .ToList();

        List<CategoryEdit> categories = names
            .Select( s => new CategoryEdit
            {
                ParentId = _seed.ParentCategoryId,
                Tier = _seed.Tier,
                Name = s,
                ApiUrl = ConvertToUrlFriendly( s ),
                ImageUrl = "default"
            } )
            .ToList();
        
        Logger.LogError( categories.Count.ToString() );

        ServiceReply<bool> reply = await BulkService.BulkInsertCategories( categories );

        if ( !reply.Success )
        {
            Logger.LogError( reply.ErrorType + reply.Message );
            SetActionMessage( false, reply.ErrorType + reply.Message );
            return;
        }

        SetActionMessage( true, "Successfully inserted categories." );
    }

    static string ConvertToUrlFriendly( string text )
    {
        // Normalize Unicode characters
        text = text.Normalize( NormalizationForm.FormKD );

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