namespace BlazorElectronics.Shared.Vendors;

public sealed class VendorsDto
{
    public Dictionary<int, List<int>> VendorIdsByCategory { get; init; } = new();
    public Dictionary<int, VendorDto> VendorsById { get; init; } = new();
}