namespace BlazorElectronics.Shared.Admin.SpecsSingle;

public sealed class UpdateSpecSingleDto
{
    public UpdateSpecSingleDto( SingleSpecLookupType specType, int specId, string? specName, Dictionary<int, object>? filterValuesById, List<int>? primaryCategories, bool? isGlobal )
    {
        SpecType = specType;
        SpecId = specId;
        SpecName = specName;
        FilterValuesById = filterValuesById;
        PrimaryCategories = primaryCategories;
        IsGlobal = isGlobal;
    }

    public SingleSpecLookupType SpecType { get; }
    public int SpecId { get; }
    public string? SpecName { get; }
    public Dictionary<int, object>? FilterValuesById { get; }
    public List<int>? PrimaryCategories { get; }
    public bool? IsGlobal { get; }
}