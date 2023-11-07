namespace BlazorElectronics.Shared.Outbound.Specs;

public struct SpecLookupValueResponse
{
    public SpecLookupValueResponse( int lookupId, string lookupValue )
    {
        SpecLookupId = lookupId;
        SpecLookupValue = lookupValue;
    }
    public int SpecLookupId { get; set; }
    public string SpecLookupValue { get; set; } = string.Empty;
}