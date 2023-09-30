namespace BlazorElectronics.Shared.DataTransferObjects.Specs;

public sealed class Spec_DTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsDynamic { get; set; } = false;
    public List<int> SpecCategoryIds { get; set; } = new();
}