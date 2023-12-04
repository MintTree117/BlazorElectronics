namespace BlazorElectronics.Shared.Features;

public class FeatureEdit : Feature, ICrudEdit
{
    public void SetId( int id )
    {
        FeatureId = id;
    }
}