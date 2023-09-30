namespace BlazorElectronics.Shared.DataTransferObjects.Specs;

public sealed class SpecFilter_DTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<object>? Values { get; set; }
}