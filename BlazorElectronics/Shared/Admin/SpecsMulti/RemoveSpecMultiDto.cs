namespace BlazorElectronics.Shared.Admin.SpecsMulti;

public sealed class RemoveSpecMultiDto
{
    public RemoveSpecMultiDto( int tableId, string? tableName )
    {
        TableId = tableId;
        TableName = tableName;
    }
    
    public int TableId { get; }
    public string? TableName { get; }
}