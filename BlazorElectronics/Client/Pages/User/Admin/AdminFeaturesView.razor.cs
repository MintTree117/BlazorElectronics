using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Features;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public partial class AdminFeaturesView : AdminView
{
    const string ERROR_GET_FEATURES_VIEW = "Failed to retireve Features View with no message!";

    [Inject] IAdminFeaturesServiceClient AdminFeaturesService { get; init; } = default!;

    FeaturesResponse _features = new();
    FeatureType _activeFeature = FeatureType.PRODUCT;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        await LoadFeaturesView();
    }

    void SetActiveFeature( FeatureType type )
    {
        _activeFeature = type;
    }
    void SortById( FeatureType list )
    {
        switch ( list )
        {
            case FeatureType.DEAL:
                _features.FeaturedDeals = _features.FeaturedDeals.OrderBy( c => c.ProductId ).ToList();
                break;
            case FeatureType.PRODUCT:
                _features.FeaturedProducts = _features.FeaturedProducts.OrderBy( c => c.ProductId ).ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException( nameof( list ), list, null );
        }

        StateHasChanged();
    }
    void SortByName( FeatureType list )
    {
        switch ( list )
        {
            case FeatureType.DEAL:
                _features.FeaturedDeals = _features.FeaturedDeals.OrderBy( c => c.ProductName ).ToList();
                break;
            case FeatureType.PRODUCT:
                _features.FeaturedProducts = _features.FeaturedProducts.OrderBy( c => c.ProductName ).ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException( nameof( list ), list, null );
        }

        StateHasChanged();
    }
    
    void CreateNewFeature( FeatureType featureType )
    {
        NavManager.NavigateTo( $"admin/features/edit?newFeature=true&featureType={featureType}" );
    }
    void EditFeaturedProduct( int id )
    {
        NavManager.NavigateTo( $"admin/features/edit?newFeature=false&featureId={id}&featureType={FeatureType.PRODUCT}" );
    }
    async Task RemoveFeature( FeatureType featureType, int featureId )
    {
        var idDto = new IntDto( featureId );

        ServiceReply<bool> result = featureType switch
        {
            FeatureType.PRODUCT => await AdminFeaturesService.RemoveFeaturedProduct( idDto ),
            FeatureType.DEAL => await AdminFeaturesService.RemoveFeaturedDeal( idDto ),
            _ => throw new ArgumentOutOfRangeException( nameof( featureType ), featureType, null )
        };

        if ( !result.Success )
        {
            SetActionMessage( false, $"Failed to delete {featureType} feature {featureId}. {result.Message}" );
            return;
        }

        SetActionMessage( true, $"Successfully deleted {featureType} feature {featureId}." );
        await LoadFeaturesView();
        StateHasChanged();
    }
    async Task LoadFeaturesView()
    {
        PageIsLoaded = false;

        ServiceReply<FeaturesResponse?> reply = await AdminFeaturesService.GetFeaturesView();

        PageIsLoaded = true;

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ?? ERROR_GET_FEATURES_VIEW );
            SetViewMessage( false, reply.Message ?? ERROR_GET_FEATURES_VIEW );
            return;
        }

        _features = reply.Data;
        ViewMessage = string.Empty;
    }
}