namespace BlazorElectronics.Shared.Admin.SpecsSingle;

public sealed class AddUpdateSpecSingleDto
{
    public AddUpdateSpecSingleDto( SingleSpecLookupType specType, int? specId, string? specName, Dictionary<int, object>? valuesById, List<int>? primaryCategories, bool? isGlobal )
    {
        SpecType = specType;
        SpecId = specId;
        SpecName = specName;
        ValuesById = valuesById;
        PrimaryCategories = primaryCategories;
        IsGlobal = isGlobal;
    }

    public SingleSpecLookupType SpecType { get; }
    public int? SpecId { get; }
    public string? SpecName { get; }
    public Dictionary<int, object>? ValuesById { get; }
    public List<int>? PrimaryCategories { get; }
    public bool? IsGlobal { get; }
}