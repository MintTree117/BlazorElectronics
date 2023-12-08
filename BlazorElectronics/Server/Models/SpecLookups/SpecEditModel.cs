namespace BlazorElectronics.Server.Models.SpecLookups;

public sealed class SpecEditModel
{
    public SpecModel? Spec { get; set; }
    public IEnumerable<SpecCategoryModel>? Categories { get; set; }
    public IEnumerable<SpecValueModel>? Values { get; set; }
}