namespace BlazorElectronics.Server.Models.SpecLookups;

public sealed class SpecLookupsModel
{
    public IEnumerable<int>? GlobalSpecs { get; set; }
    public IEnumerable<SpecLookupCategoryModel>? SpecCategories { get; set; }
    public IEnumerable<SpecLookupModel>? SpecLookups { get; set; }
    public IEnumerable<SpecLookupValueModel>? SpecValues { get; set; }
}