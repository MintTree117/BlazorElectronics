namespace BlazorElectronics.Shared.Features;

public sealed class FeatureDtoEditDto : FeatureDto, ICrudEditDto
{
    public void SetId( int id )
    {
        FeatureId = id;
    }
}