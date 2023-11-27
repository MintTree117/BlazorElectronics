using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Variants;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public partial class AdminVariantsView : AdminView
{
    const string ERROR_GET_VARIANTS_VIEW = "Failed to retireve Variant View with no message!";
    
    [Inject] IAdminVariantServiceClient AdminVariantServiceClient { get; set; } = default!;

    VariantsViewDto _dto = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        await LoadView();
    }
    
    void CreateNewVariant()
    {
        NavManager.NavigateTo( "admin/variants/edit?newVariant=true" );
    }
    void EditVariant( int variantId )
    {
        NavManager.NavigateTo( $"admin/variants/edit?newVariant=false&variantId={variantId}" );
    }
    async Task RemoveVariant( int variantId )
    {
        ApiReply<bool> reply = await AdminVariantServiceClient.Remove( new IntDto( variantId ) );

        if ( !reply.Success )
        {
            SetActionMessage( false, $"Failed to delete variant! {reply.Message}" );
            return;
        }

        SetActionMessage( true, $"Successfully deleted variant {variantId}." );
        await LoadView();
        StateHasChanged();
    }
    async Task LoadView()
    {
        PageIsLoaded = false;

        ApiReply<VariantsViewDto?> reply = await AdminVariantServiceClient.GetView();

        PageIsLoaded = true;

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ??= ERROR_GET_VARIANTS_VIEW );
            SetViewMessage( false, reply.Message ??= ERROR_GET_VARIANTS_VIEW );
            return;
        }

        _dto = reply.Data;
        ViewMessage = string.Empty;
    }

    void SortById()
    {
        _dto.Variants = _dto.Variants.OrderBy( v => v.VariantId ).ToList();
    }
    void SortByCategory()
    {
        _dto.Variants = _dto.Variants.OrderBy( v => v.PrimaryCategoryId ).ToList();
    }
    void SortByName()
    {
        _dto.Variants = _dto.Variants.OrderBy( v => v.VariantName ).ToList();
    }
}