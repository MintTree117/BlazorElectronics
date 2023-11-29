using System.Collections.Specialized;
using System.Web;
using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Variants;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public partial class AdminVariantsEdit : AdminView
{
    [Inject] IAdminVariantServiceClient AdminVariantServiceClient { get; set; } = default!;

    bool _newVariant;
    VariantEditDto _dto = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        if ( !TryParseUrlParameters( out int variantId ) )
        {
            SetViewMessage( false, ERROR_INVALID_URL_PARAMS );
            Logger.LogError( ERROR_INVALID_URL_PARAMS );
            StartPageRedirection();
            return;
        }

        if ( _newVariant )
        {
            PageIsLoaded = true;
            return;
        }
        
        ApiReply<VariantEditDto?> reply = await AdminVariantServiceClient.GetEdit( new IntDto( variantId ) );

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message );
            SetViewMessage( false, reply.Message );
            StartPageRedirection();
            return;
        }

        PageIsLoaded = true;
        _dto = reply.Data;
        StateHasChanged();
    }
    bool TryParseUrlParameters( out int variantId )
    {
        variantId = -1;

        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        NameValueCollection queryString = HttpUtility.ParseQueryString( uri.Query );

        string? newVariantString = queryString.Get( "newVariant" );
        string? variantIdString = queryString.Get( "variantId" );

        bool parsed = !string.IsNullOrWhiteSpace( newVariantString ) &&
                      bool.TryParse( newVariantString, out _newVariant );
        
        if ( _newVariant )
            return parsed;

        return !string.IsNullOrWhiteSpace( variantIdString ) && int.TryParse( variantIdString, out variantId );
    }
    
    async Task Submit()
    {
        if ( _newVariant )
            await SubmitNew();
        else
            await SubmitEdit();
    }
    async Task SubmitNew()
    {
        ApiReply<int> reply = await AdminVariantServiceClient.Add( _dto );

        if ( !reply.Success )
        {
            SetActionMessage( false, $"Failed to add variant! {reply.Message}" );
            return;
        }

        _newVariant = false;
        _dto.VariantId = reply.Data;

        SetActionMessage( true, $"Successfully added variant." );
        StateHasChanged();
    }
    async Task SubmitEdit()
    {
        ApiReply<bool> reply = await AdminVariantServiceClient.Update( _dto );

        if ( !reply.Success )
        {
            SetActionMessage( false, $"Failed to update variant! {reply.Message}" );
            return;
        }

        SetActionMessage( true, $"Successfully updated variant." );
        StateHasChanged();
    }
}