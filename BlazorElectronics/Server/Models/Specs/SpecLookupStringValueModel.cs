namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupStringValueModel
{
    public SpecLookupStringValueModel( short specId, short specValueId, string specValue )
    {
        SpecId = specId;
        SpecValueId = specValueId;
        SpecValue = specValue;
    }

    public short SpecId { get; }
    public short SpecValueId { get; }
    public string SpecValue { get; }
}