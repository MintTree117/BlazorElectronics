namespace BlazorElectronics.Shared.Inbound.Admin.SpecLookups;

public class AddSpecMultiTableRequest : AdminRequest
{
    public string? TableName { get; set; }
    public List<string>? MultiValues { get; set; }
    public List<int>? PrimaryCategories { get; set; }
    public bool IsGlobal { get; set; }
}