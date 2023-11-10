namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupMultiTableModel
{
    public short TableId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}