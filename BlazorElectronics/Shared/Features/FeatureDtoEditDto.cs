namespace BlazorElectronics.Shared.Features;

public class FeatureDtoEditDto : FeatureDto, ICrudEditDto
{
    public void SetId( int id )
    {
        FeatureId = id;
    }
}