namespace BlazorElectronics.Shared.Vendors;

public sealed class VendorsResponse
{
    public Dictionary<int, List<int>> VendorIdsByCategory { get; init; } = new();
    public Dictionary<int, Vendor> VendorsById { get; init; } = new();
}