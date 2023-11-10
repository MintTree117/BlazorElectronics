namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupValuesModel
{
    public Dictionary<int, IEnumerable<SpecLookupValueModel>?>? DyanmicValuesByTableId { get; set; }
}