namespace BlazorElectronics.Shared.Admin.Products;

public sealed class AddUpdateProductSpecMultiDto
{
    public AddUpdateProductSpecMultiDto( string? tableName, int specId, string? value )
    {
        TableName = tableName;
        SpecId = specId;
        Value = value;
    }
    
    public string? TableName { get; }
    public int SpecId { get; }
    public string? Value { get; }
}