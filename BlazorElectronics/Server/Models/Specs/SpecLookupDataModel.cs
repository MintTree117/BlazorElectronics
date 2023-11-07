namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupDataModel
{
    public Dictionary<int, IEnumerable<DynamicSpecValue>?>? DyanmicValuesByTableId { get; set; }
}