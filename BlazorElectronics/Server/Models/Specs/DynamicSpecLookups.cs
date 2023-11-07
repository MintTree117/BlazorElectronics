namespace BlazorElectronics.Server.Models.Specs;

public sealed class DynamicSpecLookups
{
    public Dictionary<int, IEnumerable<DynamicSpecLookup>> SpecLookups { get; set; } = new();
}