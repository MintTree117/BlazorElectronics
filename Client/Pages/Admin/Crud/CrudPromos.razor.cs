using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Client.Pages.Admin.Crud;

public partial class CrudPromos : CrudPage<PromoEditDto, PromoEditDto>
{
    protected override async Task OnInitializedAsync()
    {
        ItemTitle = "Promo";

        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        ApiPath = "api/AdminPromo";
        await LoadView();
    }
    protected override void GenerateTableMeta()
    {
        base.GenerateTableMeta();
        THeadMeta.Add( "Promo Discount", SortByDiscount );
    }

    void SortByDiscount()
    {
        ItemsView = ItemsView.OrderBy( p => p.PromoDiscount ).ToList();
    }
}