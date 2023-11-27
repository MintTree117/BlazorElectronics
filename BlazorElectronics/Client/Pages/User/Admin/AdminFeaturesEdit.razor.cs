using System.Collections.Specialized;
using System.Web;
using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Features;
using BlazorElectronics.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public partial class AdminFeaturesEdit : AdminView
{
    [Inject] IAdminFeaturesServiceClient AdminFeaturesServiceClient { get; set; } = default!;

    bool _newFeature;
    FeatureType _featureType;
    int _dealProductId;
    FeaturedProductEditDto _productDto = new();
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        if ( !TryParseUrlParameters( out int featureId ) )
        {
            SetViewMessage( false, ERROR_INVALID_URL_PARAMS );
            Logger.LogError( ERROR_INVALID_URL_PARAMS );
            StartPageRedirection();
            return;
        }

        if ( _newFeature )
        {
            PageIsLoaded = true;
            return;
        }

        ApiReply<FeaturedProductEditDto?> reply = await AdminFeaturesServiceClient.GetFeaturedProductEdit( new IntDto( featureId ) );

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ??= "Failed to get feature!" );
            SetViewMessage( false, reply.Message ??= "Failed to get feature!" );
            StartPageRedirection();
            return;
        }

        PageIsLoaded = true;
        _productDto = reply.Data;
        StateHasChanged();
    }
    bool TryParseUrlParameters( out int featureId )
    {
        featureId = -1;

        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        NameValueCollection queryString = HttpUtility.ParseQueryString( uri.Query );

        string? newVariantString = queryString.Get( "newFeature" );
        string? featureIdString = queryString.Get( "featureId" );
        string? featureTypeString = queryString.Get( "featureType" );

        bool parsed = !string.IsNullOrWhiteSpace( newVariantString ) &&
                      bool.TryParse( newVariantString, out _newFeature ) &&
                      !string.IsNullOrWhiteSpace( featureTypeString ) &&
                      Enum.TryParse( featureTypeString, out _featureType );
        
        if ( _newFeature )
            return parsed;

        return !string.IsNullOrWhiteSpace( featureIdString ) && int.TryParse( featureIdString, out featureId );
    }

    async Task Submit()
    {
        if ( _newFeature )
            await SubmitNew();
        else
            await SubmitEdit();
    }
    async Task SubmitNew()
    {
        ApiReply<bool> reply = _featureType == FeatureType.DEAL
            ? await AdminFeaturesServiceClient.AddFeaturedDeal( new IntDto( _dealProductId ) )
            : await AdminFeaturesServiceClient.AddFeaturedProduct( _productDto );
        
        if ( !reply.Success )
        {
            SetActionMessage( false, $"Failed to add feature! {reply.Message}" );
            return;
        }

        _newFeature = false;
        
        SetActionMessage( true, $"Successfully added feature." );
        StateHasChanged();
    }
    async Task SubmitEdit()
    {
        if ( _featureType == FeatureType.DEAL )
        {
            Logger.LogError( "Invalid feature type!" );
            SetActionMessage( false, "Invalid feature type!" );
            return;
        }
        
        ApiReply<bool> reply = await AdminFeaturesServiceClient.UpdateFeaturedProduct( _productDto );

        if ( !reply.Success )
        {
            SetActionMessage( false, $"Failed to update feature! {reply.Message}" );
            return;
        }

        SetActionMessage( true, $"Successfully updated feature." );
        StateHasChanged();
    }
}