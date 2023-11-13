namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupMultiTableModel
{
    public SpecLookupMultiTableModel( short tableId, string tableName, string displayName )
    {
        TableId = tableId;
        TableName = tableName;
        DisplayName = displayName;
    }
    
    public short TableId { get; }
    public string TableName { get; }
    public string DisplayName { get; }
}