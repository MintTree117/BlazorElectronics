namespace BlazorElectronics.Server.Models.Specs;

public sealed class DynamicSpecLookupValuesModel
{
    public Dictionary<int, IEnumerable<DynamicSpecValueModel>?>? DyanmicValuesByTableId { get; set; }
}