namespace BlazorElectronics.Shared.DataTransferObjects.Specs;

public sealed class Spec_DTO
{
    public int Id { get; set; }
    public Type? DataType { get; set; }
    public bool IsRaw { get; set; } = false;
    public string? Name { get; set; }
    public List<int> SpecCategoryIds { get; set; } = new();
    public List<int> SpecFilterIds { get; set; } = new();
}