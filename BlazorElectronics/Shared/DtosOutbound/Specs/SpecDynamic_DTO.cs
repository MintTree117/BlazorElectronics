namespace BlazorElectronics.Shared.DataTransferObjects.Specs;

public sealed class SpecDynamic_DTO
{
    public int Id { get; set; }
    public int DataType { get; set; }
    public int SpecType { get; set; }
    public string? Name { get; set; }
    public List<object>? Values { get; set; }
}