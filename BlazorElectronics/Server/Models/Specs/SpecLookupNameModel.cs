namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupNameModel
{
    public SpecLookupNameModel( short specId, string specName )
    {
        SpecId = specId;
        SpecName = specName;
    }
    
    public short SpecId { get; }
    public string SpecName { get; }
}