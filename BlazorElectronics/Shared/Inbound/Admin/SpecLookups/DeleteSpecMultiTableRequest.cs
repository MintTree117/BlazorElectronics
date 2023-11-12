namespace BlazorElectronics.Shared.Inbound.Admin.SpecLookups;

public sealed class DeleteSpecMultiTableRequest : AdminRequest
{
    public DeleteSpecMultiTableRequest( int tableId, string? tableName )
    {
        TableId = tableId;
        TableName = tableName;
    }
    
    public int TableId { get; }
    public string? TableName { get; }
}