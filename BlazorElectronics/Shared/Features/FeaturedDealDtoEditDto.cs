namespace BlazorElectronics.Shared.Features;

public sealed class FeaturedDealDtoEditDto : FeaturedDealDto, ICrudEditDto
{
    public void SetId( int id )
    {
        ProductId = id;
    }
}