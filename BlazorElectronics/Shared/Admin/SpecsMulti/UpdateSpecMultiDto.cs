namespace BlazorElectronics.Shared.Admin.SpecsMulti;

public sealed class UpdateSpecMultiDto
{
    public UpdateSpecMultiDto( int tableId, string? tableName, List<string>? multiValues, List<int>? primaryCategories, bool? isGlobal )
    {
        TableId = tableId;
        TableName = tableName;
        MultiValues = multiValues;
        PrimaryCategories = primaryCategories;
        IsGlobal = isGlobal;
    }

    public int TableId { get; }
    public string? TableName { get; }
    public List<string>? MultiValues { get; }
    public List<int>? PrimaryCategories { get; }
    public bool? IsGlobal { get; }
}