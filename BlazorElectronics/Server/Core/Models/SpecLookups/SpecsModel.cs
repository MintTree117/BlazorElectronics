namespace BlazorElectronics.Server.Core.Models.SpecLookups;

public sealed class SpecsModel
{
    public IEnumerable<SpecCategoryModel>? SpecCategories { get; set; }
    public IEnumerable<SpecModel>? Specs { get; set; }
    public IEnumerable<SpecValueModel>? SpecValues { get; set; }
}