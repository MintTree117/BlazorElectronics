namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupIntFilterModel
{
    public SpecLookupIntFilterModel( short specId, short filterId, string filterValue )
    {
        SpecId = specId;
        FilterId = filterId;
        FilterValue = filterValue;
    }
    
    public short SpecId { get; }
    public short FilterId { get; }
    public string FilterValue { get; }
}