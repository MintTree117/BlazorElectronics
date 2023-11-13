namespace BlazorElectronics.Shared.Admin.SpecsMulti;

public sealed class UpdateSpecMultiDto
{
    public UpdateSpecMultiDto( int? tableId, List<string>? multiValues, List<int>? primaryCategories, bool? isGlobal )
    {
        TableId = tableId;
        MultiValues = multiValues;
        PrimaryCategories = primaryCategories;
        IsGlobal = isGlobal;
    }

    public int? TableId { get; }
    public List<string>? MultiValues { get; }
    public List<int>? PrimaryCategories { get; }
    public bool? IsGlobal { get; }
}