namespace BlazorElectronics.Shared.Outbound.Specs;

public sealed class SpecLookupTableResponse
{
    public SpecLookupTableResponse( int tableId, string diplayName, List<SpecLookupValueResponse> lookups )
    {
        TableId = tableId;
        TableDisplayName = diplayName;
        TableLookups = lookups;
    }
    
    public int TableId { get; set; }
    public string TableDisplayName { get; set; }
    public List<SpecLookupValueResponse> TableLookups { get; set; }
}