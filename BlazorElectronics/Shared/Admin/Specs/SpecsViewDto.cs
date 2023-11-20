namespace BlazorElectronics.Shared.Admin.Specs;

public sealed class SpecsViewDto
{
    public List<SpecView> IntSpecs { get; set; } = new();
    public List<SpecView> StringSpecs { get; set; } = new();
    public List<SpecView> BoolSpecs { get; set; } = new();
    public List<SpecView> MultiSpecs { get; set; } = new();
}