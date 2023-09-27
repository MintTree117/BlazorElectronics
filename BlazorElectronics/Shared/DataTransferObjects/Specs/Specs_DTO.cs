namespace BlazorElectronics.Shared.DataTransferObjects.Specs;

public sealed class Specs_DTO
{
    public Dictionary<int, Spec_DTO> SpecsById { get; set; } = new();
    public Dictionary<string, int> IdsByName { get; set; } = new();
}