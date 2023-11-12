namespace BlazorElectronics.Shared.Inbound.Admin.SpecLookups;

public class AddSpecSingleRequest : AdminRequest
{
    public SingleSpecLookupType SpecType { get; set; }
    public string? SpecName { get; set; }
    public Dictionary<int, object>? FilterValuesById { get; set; }
    public List<int>? PrimaryCategories { get; set; }
    public bool IsGlobal { get; set; }
}