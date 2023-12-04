namespace BlazorElectronics.Shared.Features;

public sealed class FeaturedDealEdit : FeaturedDeal, ICrudEdit
{
    public void SetId( int id )
    {
        ProductId = id;
    }
}