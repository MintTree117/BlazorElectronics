namespace BlazorElectronics.Server.Models.Specs;

public sealed class DynamicSpecTableMetaModel
{
    public short LookupTableId { get; set; }
    public string LookupTableName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}