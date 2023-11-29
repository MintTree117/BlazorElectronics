namespace BlazorElectronics.Server.Models.SpecLookups;

public sealed class SpecLookupEditModel
{
    public SpecLookupModel? Spec { get; set; }
    public bool IsGlobal { get; set; }
    public IEnumerable<SpecLookupCategoryModel>? Categories { get; set; }
    public IEnumerable<SpecLookupValueModel>? Values { get; set; }
}