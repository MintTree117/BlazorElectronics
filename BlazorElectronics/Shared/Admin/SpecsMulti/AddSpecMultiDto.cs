namespace BlazorElectronics.Shared.Admin.SpecsMulti;

public sealed class AddSpecMultiDto
{
    public AddSpecMultiDto( string? tableName, List<string>? multiValues, List<int>? primaryCategories, bool? isGlobal )
    {
        TableName = tableName;
        MultiValues = multiValues;
        PrimaryCategories = primaryCategories;
        IsGlobal = isGlobal;
    }
    public string? TableName { get; }
    public List<string>? MultiValues { get; }
    public List<int>? PrimaryCategories { get; }
    public bool? IsGlobal { get; }
}