namespace BlazorElectronics.Shared.Admin.Products;

public sealed class AddUpdateProductSpecsDynamicDto
{
    public AddUpdateProductSpecsDynamicDto( string? tableName, List<string>? values )
    {
        TableName = tableName;
        Values = values;
    }
    
    public string? TableName { get; }
    public List<string>? Values { get; }
}