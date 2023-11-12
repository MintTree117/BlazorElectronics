namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupValuesModel
{
    public IEnumerable<IEnumerable<string>?>? ValuesByTable { get; set; }
}