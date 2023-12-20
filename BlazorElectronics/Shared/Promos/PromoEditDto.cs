namespace BlazorElectronics.Shared.Promos;

public class PromoEditDto : CrudViewDto, ICrudEditDto
{
    public float PromoDiscount { get; set; }

    public void SetId( int id )
    {
        Id = id;
    }
}