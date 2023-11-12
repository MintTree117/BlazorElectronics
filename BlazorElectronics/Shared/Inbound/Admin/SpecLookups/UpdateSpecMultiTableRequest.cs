namespace BlazorElectronics.Shared.Inbound.Admin.SpecLookups;

public sealed class UpdateSpecMultiTableRequest : AddSpecMultiTableRequest
{
    public UpdateSpecMultiTableRequest( string? tableName, int tableId, List<string>? multiValues, List<int>? primaryCategories, bool isGlobal )
        : base( tableName, multiValues, primaryCategories, isGlobal )
    {
        TableId = tableId;
    }

    public int TableId { get; }
}