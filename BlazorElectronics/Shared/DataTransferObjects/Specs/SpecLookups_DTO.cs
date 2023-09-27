namespace BlazorElectronics.Shared.DataTransferObjects.Specs;

public sealed class SpecLookups_DTO
{
    public Dictionary<int, List<object>> LookupValuesBySpecId { get; set; } = new();
}