namespace BlazorElectronics.Shared.Admin.SpecsSingle;

public sealed class AddSpecSingleDto
{
    public AddSpecSingleDto( SingleSpecLookupType specType, string? specName, Dictionary<int, object>? filterValuesById, List<int>? primaryCategories, bool? isGlobal )
    {
        SpecType = specType;
        SpecName = specName;
        FilterValuesById = filterValuesById;
        PrimaryCategories = primaryCategories;
        IsGlobal = isGlobal;
    }
    
    public SingleSpecLookupType SpecType { get; }
    public string? SpecName { get; }
    public Dictionary<int, object>? FilterValuesById { get; }
    public List<int>? PrimaryCategories { get; }
    public bool? IsGlobal { get; }
}